﻿using Amazon.S3.Model;
using DBADash;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using static DBADash.DBADashConnection;
using Serilog;
using SerilogTimings;
using System.Collections.Generic;

namespace DBADashService
{
    [DisallowConcurrentExecution, PersistJobDataAfterExecution]
    public class DBADashJob : IJob
    {
        static readonly CollectionConfig config = SchedulerServiceConfig.Config;

        static string GetID(DataSet ds)
        {
            return ds.Tables["DBADash"].Rows[0]["Instance"] + "_" + ds.Tables["DBADash"].Rows[0]["DBName"];
        }

        /// <summary>
        /// Parse Instance from filename.  File format is DBADash_YYYYMMDD_HHMM_SS_{InstanceName}_{random}.xml
        /// </summary>
        public static string ParseInstance(string fileName)
        {
            return fileName[25..fileName.LastIndexOf("_")];
        }

        public Task Execute(IJobExecutionContext context)
        {
            Log.Information("Processing Job : " + context.JobDetail.Key);
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var cfg = JsonConvert.DeserializeObject<DBADashSource>(dataMap.GetString("CFG"));
          
            try
            {
                if (cfg.SourceConnection.Type == ConnectionType.Directory)
                {
                    Log.Debug("Wait for lock {0}", context.JobDetail.Key);
                    // Ensures that this folder can only be processed by 1 job instance at a time.
                    // Note: DisallowConcurrentExecution didn't prevent triggered at startup job from overlapping with the scheduled one
                    lock (Program.Locker.GetLock(cfg.ConnectionString))
                    {
                        Log.Debug("Lock acquired {0}", context.JobDetail.Key);
                        CollectFolder(cfg);
                    }

                }
                else if (cfg.SourceConnection.Type == ConnectionType.AWSS3)
                {
                    Log.Debug("Wait for lock {0}", context.JobDetail.Key);
                    // Ensures that S3 folder can only be processed by 1 job instance at a time.
                    // Note: DisallowConcurrentExecution didn't prevent triggered at startup job from overlapping with the scheduled one
                    lock (Program.Locker.GetLock(cfg.ConnectionString)) 
                    {
                        Log.Debug("Lock acquired {0}", context.JobDetail.Key);
                        CollectS3(cfg);
                    }
                }
                else
                {
                    CollectSQL(cfg,dataMap,context);                
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "JobExecute");
            }

            return Task.CompletedTask;

        }

        /// <summary>
        /// Collect data from monitored SQL instance
        /// </summary>
        private static void CollectSQL(DBADashSource cfg, JobDataMap dataMap, IJobExecutionContext context)
        {
            var types = JsonConvert.DeserializeObject<CollectionType[]>(dataMap.GetString("Type"));
            try
            {
                // Value used to disable future collections of SlowQueries if we encounter a not supported error on a RDS instance not running Standard or Enterprise edition
                bool dataMapExtendedEventsNotSupported = dataMap.GetBooleanValue("IsExtendedEventsNotSupportedException");
                var collector = new DBCollector(cfg, config.ServiceName)
                {
                    Job_instance_id = dataMap.GetInt("Job_instance_id"),
                    IsExtendedEventsNotSupportedException = dataMapExtendedEventsNotSupported,

                };
                if (SchedulerServiceConfig.Config.IdentityCollectionThreshold.HasValue)
                {
                    collector.IdentityCollectionThreshold = (int)SchedulerServiceConfig.Config.IdentityCollectionThreshold;
                }

                var jobLastCollected = dataMap.GetDateTime("JobCollectDate");

                // Setting the JobLastModified means we will only collect job data if jobs have been updated since the last collection.
                // This won't detect all changes - like changes to schedules.  Skip setting JobLastModified if we haven't collected in 1 day to ensure we collect at least once per day
                if (DateTime.UtcNow.Subtract(jobLastCollected).TotalMinutes < 1430) // Allow 10min
                {
                    collector.JobLastModified = dataMap.GetDateTime("JobLastModified");
                }

                if (context.PreviousFireTimeUtc.HasValue)
                {
                    collector.PerformanceCollectionPeriodMins = (Int32)DateTime.UtcNow.Subtract(context.PreviousFireTimeUtc.Value.UtcDateTime).TotalMinutes + 5;
                }
                else
                {
                    collector.PerformanceCollectionPeriodMins = 30;
                }
                collector.LogInternalPerformanceCounters = SchedulerServiceConfig.Config.LogInternalPerformanceCounters;
                using (var op = Operation.Begin("Collect {types} from instance {instance}", string.Join(", ", types.Select(s => s.ToString()).ToArray()), cfg.SourceConnection.ConnectionForPrint))
                {
                    collector.Collect(types);
                    if (!dataMapExtendedEventsNotSupported && collector.IsExtendedEventsNotSupportedException)
                    {
                        // We encounterd an error setting up extended events on a RDS instance because it's only supported for Standard and Enterprise editions.  Disable the collection
                        Log.Information("Disabling Extended events collection for {0}.  Instance type doesn't support extended events", cfg.SourceConnection.ConnectionForPrint);
                        dataMap.Put("IsExtendedEventsNotSupportedException", true);
                    }
                    op.Complete();
                }
                bool containsJobs = collector.Data.Tables.Contains("Jobs");
                string fileName = cfg.GenerateFileName(cfg.SourceConnection.ConnectionForFileName);
                try
                {
                    DestinationHandling.WriteAllDestinations(collector.Data, cfg, fileName).Wait();
                    dataMap.Put("Job_instance_id", collector.Job_instance_id); // Store instance_id so we can get new history only on next run
                    if (containsJobs)
                    {
                        // We have collected jobs data - Store JobLastModified and time we have collected the jobs.
                        // Used on next run to determine if we need to refresh this data.
                        dataMap.Put("JobLastModified", collector.JobLastModified);
                        dataMap.Put("JobCollectDate", DateTime.UtcNow);
                    }
                    collector.CacheCollectedText();
                    collector.CacheCollectedPlans();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error writing {filename} to destination.  File will be copied to {folder}", fileName, SchedulerServiceConfig.FailedMessageFolder);
                    DestinationHandling.WriteFolder(collector.Data, SchedulerServiceConfig.FailedMessageFolder, fileName);
                }

            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error collecting types {types} from instance {instance}", string.Join(", ", types.Select(s => s.ToString()).ToArray()), cfg.SourceConnection.ConnectionForPrint);
            }
        }

        /// <summary>
        /// Split file list by Instance parsed from the filename.  Each instance will have 1 item in the dictionary containing a list of files to process for that instance
        /// </summary>
        private static Dictionary<string, List<string>> GetFilesToProcessByInstance(List<string> files)
        {
            Dictionary<string, List<string>> filesToProcessByInstance = new();
            foreach (var path in files)
            {
                string instance;
                try
                {
                    instance = ParseInstance(Path.GetFileName(path));
                }
                catch (Exception ex)
                {
                    instance = "default";
                    Log.Warning("Unable to parse Instance from {0}: {1}", path, ex.Message);
                }
                if (filesToProcessByInstance.ContainsKey(instance))
                {
                    filesToProcessByInstance[instance].Add(path);
                }
                else
                {
                    filesToProcessByInstance.Add(instance, new() { path });
                }
            }
            return filesToProcessByInstance;
        }

        /// <summary>
        /// Get files to import from folder and process in parallel for each instance.
        /// </summary>
        private static void CollectFolder(DBADashSource cfg)
        {
            string folder = cfg.GetSource();
            Log.Logger.Information("Import from folder {folder}", folder);
            if (System.IO.Directory.Exists(folder))
            {
                try
                {
                    var files = System.IO.Directory.EnumerateFiles(folder, "DBADash_*", SearchOption.TopDirectoryOnly).Where(f => f.EndsWith(".xml")).ToList();

                    Dictionary<string, List<string>> filesToProcessByInstance = GetFilesToProcessByInstance(files);
                    // Parallel processing of files for each instance, but process the files for a given instance in order
                    Parallel.ForEach(filesToProcessByInstance, instanceItem =>
                    {
                        List<string> instanceFiles = instanceItem.Value;
                        ProcessFileListForCollectFolder(instanceFiles, cfg);
                        
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Import from folder {folder}", folder);
                }
            }
            else
            {
                Log.Error("Source directory doesn't exist {folder}", folder);
            }
        }

        /// <summary>
        /// Process a given list of files in order for a specific instance, writing collected data to the DBADash repository database
        /// </summary>
        static void ProcessFileListForCollectFolder(List<string>files, DBADashSource cfg)
        {
            files.Sort(); // Ensure we process files in order
            foreach (string f in files)
            {
                Log.Information("Processing file {0}", f);
                string fileName = Path.GetFileName(f);
                try
                {
                    var ds = DataSetSerialization.DeserializeFromFile(f);
                    lock (Program.Locker.GetLock(GetID(ds)))
                    {
                        DestinationHandling.WriteAllDestinations(ds, cfg, fileName).Wait();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error importing from {filename}.  File will be copied to {failedmessagefolder}", fileName, SchedulerServiceConfig.FailedMessageFolder);
                    File.Copy(f, Path.Combine(SchedulerServiceConfig.FailedMessageFolder, f));
                }
                finally
                {
                    System.IO.File.Delete(f);
                }
            }
        }

        /// <summary>
        /// Process The S3 bucket source.  Run a separate thread per instance and process the files for each instance sequentially in the order they were collected
        /// </summary>
        private static void CollectS3(DBADashSource cfg)
        {
            Log.Information("Import from S3 {connection}", cfg.ConnectionString);
            try
            {
                var uri = new Amazon.S3.Util.AmazonS3Uri(cfg.ConnectionString);
                using var s3Cli = AWSTools.GetAWSClient(config.AWSProfile, config.AccessKey, config.GetSecretKey(), uri);
                ListObjectsRequest request = new() { BucketName = uri.Bucket, Prefix = (uri.Key + "/DBADash_").Replace("//", "/") };
       
                do
                {
                    ListObjectsResponse resp;
                    using (var listObjectsTask = s3Cli.ListObjectsAsync(request))
                    {
                        listObjectsTask.Wait();
                        resp = listObjectsTask.Result;
                    }

                    List<string> fileList = resp.S3Objects.Where(f => f.Key.EndsWith(".xml")).Select(f => f.Key).ToList();
                    Dictionary<string, List<string>> filesToProcessByInstance = GetFilesToProcessByInstance(fileList);

                    Log.Information("Processing {0} files from {1}. Instance Count: {2}", resp.S3Objects.Count, uri.Key, filesToProcessByInstance.Count);

                    // Start a thread to process the files associated with each instance.  Each instance will have it's files processed sequentially in the order they were collected.
                    Parallel.ForEach(filesToProcessByInstance, instanceItem =>
                    {
                        List<string> instanceFiles = instanceItem.Value;
                        ProcessS3FileListForCollectS3(instanceFiles, s3Cli,uri,cfg);
                  
                    });
                    if (resp.IsTruncated)
                    {
                        Log.Debug("Response truncated.  Processing next marker for {0}", uri.Key);
                        request.Marker = resp.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }

                }
                while (request != null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error importing files from S3");
            }
        }

        /// <summary>
        /// Process a given list of S3 files for a sepecific instance in order, writing collected data to DBA Dash repository database
        /// </summary>
        private static void ProcessS3FileListForCollectS3(List<string>instanceFiles,Amazon.S3.AmazonS3Client s3Cli, Amazon.S3.Util.AmazonS3Uri uri, DBADashSource cfg)
        {
            instanceFiles.Sort(); // Ensure files are processed in order
            foreach (string s3Path in instanceFiles)
            {
                using var getObjectTask = s3Cli.GetObjectAsync(uri.Bucket, s3Path);
                getObjectTask.Wait();

                using GetObjectResponse response = getObjectTask.Result;
                using Stream responseStream = response.ResponseStream;
                DataSet ds;

                ds = new DataSet();
                ds.ReadXml(responseStream);

                lock (Program.Locker.GetLock(GetID(ds))) // Ensures we process 1 item at a time for each instance
                {
                    string fileName = Path.GetFileName(s3Path);
                    try
                    {
                        DestinationHandling.WriteAllDestinations(ds, cfg, fileName).Wait();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error importing file {filename}.  Writing file to failed message folder {folder}", fileName, SchedulerServiceConfig.FailedMessageFolder);
                        DestinationHandling.WriteFolder(ds, SchedulerServiceConfig.FailedMessageFolder, fileName);
                    }
                    finally
                    {
                        using Task<DeleteObjectResponse> deleteTask = s3Cli.DeleteObjectAsync(uri.Bucket, s3Path);
                        deleteTask.Wait();
                    }
                }
                Log.Information("Imported {file}", s3Path);
            }
        }


    }

}
