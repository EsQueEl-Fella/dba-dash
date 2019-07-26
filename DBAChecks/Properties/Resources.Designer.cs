﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DBAChecks.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DBAChecks.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WITH T
        ///AS (SELECT j.job_id,
        ///           j.name,
        ///           MAX(   CASE
        ///                      WHEN jh.step_id = 0
        ///                      AND  jh.run_status &lt;&gt; 1 THEN
        ///                          dt.RunDateTime
        ///                      ELSE
        ///                          NULL
        ///                  END
        ///              ) LastFail,
        ///           MAX(   CASE
        ///                      WHEN jh.step_id = 0
        ///                      AND  jh.run_status = 1 THEN
        ///                          dt.RunDateTime
        ///                      ELSE
        ///    [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLAgentJobSummary {
            get {
                return ResourceManager.GetString("SQLAgentJobSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select database_name,type, MAX(backup_start_date) LastBackup
        ///from msdb.dbo.backupset
        ///where server_name=@@SERVERNAME
        ///AND backup_finish_date&gt;=DATEADD(d,-10,GETUTCDATE())
        ///group by database_name, type.
        /// </summary>
        internal static string SQLBackups {
            get {
                return ResourceManager.GetString("SQLBackups", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT configuration_id,CAST(value as BIGINT) as value,CAST(value_in_use as BIGINT) as value_in_use
        ///FROM sys.configurations.
        /// </summary>
        internal static string SQLConfigurations {
            get {
                return ResourceManager.GetString("SQLConfigurations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @Corruption TABLE(
        ///		SourceTable TINYINT,
        ///		database_id INT NOT NULL,
        ///		last_update_date DATETIME NOT NULL
        ///)
        ///IF OBJECT_ID(&apos;msdb.dbo.suspect_pages&apos;) IS NOT NULL
        ///BEGIN
        ///	INSERT INTO @Corruption(SourceTable,database_id,last_update_date)
        ///	SELECT CAST(1 AS TINYINT) AS SourceTable,
        ///		   database_id,
        ///		   MAX(last_update_date) last_update_date
        ///	FROM msdb.dbo.suspect_pages
        ///	GROUP BY database_id
        ///END
        ///IF OBJECT_ID(&apos;msdb.sys.dm_db_mirroring_auto_page_repair&apos;) IS NOT NULL
        ///BEGIN
        ///	INSERT INTO @Corru [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLCorruption {
            get {
                return ResourceManager.GetString("SQLCorruption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE #sysdb(
        ///	[name] [sysname] NOT NULL,
        ///	[database_id] [int] NOT NULL,
        ///	[source_database_id] [int] NULL,
        ///	[owner_sid] [varbinary](85) NULL,
        ///	[create_date] [datetime] NOT NULL,
        ///	[compatibility_level] [tinyint] NOT NULL,
        ///	[collation_name] [sysname] NULL,
        ///	[user_access] [tinyint] NULL,
        ///	[is_read_only] [bit] NULL,
        ///	[is_auto_close_on] [bit] NOT NULL,
        ///	[is_auto_shrink_on] [bit] NULL,
        ///	[state] [tinyint] NULL,
        ///	[is_in_standby] [bit] NULL,
        ///	[is_cleanly_shutdown] [bit] NULL,
        ///	[is_supplemental_ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLDatabases {
            get {
                return ResourceManager.GetString("SQLDatabases", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @DBName SYSNAME
        ///DECLARE @SQL NVARCHAR(MAX)
        ///CREATE TABLE #DBConfig( 
        ///	database_id INT NOT NULL,
        ///	configuration_id INT NOT NULL,
        ///	name NVARCHAR(60) NOT NULL,
        ///	value NVARCHAR(128) NULL,
        ///	value_for_secondary NVARCHAR(128) NULL,
        ///	PRIMARY KEY (database_id,configuration_id)
        ///)
        ///
        ///IF OBJECT_ID(&apos;sys.database_scoped_configurations&apos;) IS NOT NULL
        ///BEGIN
        ///	DECLARE DBs CURSOR FAST_FORWARD READ_ONLY FOR
        ///	SELECT name
        ///	FROM sys.databases
        ///	WHERE state  = 0
        ///	AND DATABASEPROPERTYEX(name, &apos;Updateability&apos;) =  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLDBConfig {
            get {
                return ResourceManager.GetString("SQLDBConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF OBJECT_ID(&apos;sys.dm_os_volume_stats&apos;) IS NOT NULL
        ///BEGIN
        ///	SELECT DISTINCT dovs.volume_mount_point AS Name,
        ///		dovs.total_bytes as Capacity,
        ///		dovs.available_bytes as FreeSpace,
        ///		dovs.logical_volume_name as Label
        ///	FROM sys.master_files mf
        ///	CROSS APPLY sys.dm_os_volume_stats(mf.database_id, mf.FILE_ID) dovs
        ///END.
        /// </summary>
        internal static string SQLDrives {
            get {
                return ResourceManager.GetString("SQLDrives", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @ProcessorNameString NVARCHAR(512)
        ///DECLARE @SystemManufacturer NVARCHAR(512)
        ///DECLARE @SystemProductName NVARCHAR(512)
        ///DECLARE @IsAgentRunning BIT
        ///DECLARE @InstantFileInitializationEnabled BIT
        ///IF OBJECT_ID(&apos;sys.xp_instance_regread&apos;) IS NOT NULL AND EXISTS(SELECT * FROM fn_my_permissions ( &apos;sys.xp_instance_regread&apos;, &apos;OBJECT&apos; ) WHERE permission_name=&apos;EXECUTE&apos;)
        ///BEGIN  
        ///	EXEC sys.xp_instance_regread N&apos;HKEY_LOCAL_MACHINE&apos;, N&apos;HARDWARE\DESCRIPTION\System\CentralProcessor\0&apos;, N&apos;ProcessorNameString&apos;,@P [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLExtraProperties {
            get {
                return ResourceManager.GetString("SQLExtraProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @DBName SYSNAME
        ///DECLARE @SQL NVARCHAR(MAX)
        ///CREATE TABLE #FileList ( 
        ///database_id INT,
        ///file_id INT,
        ///data_space_id INT,
        ///name SYSNAME,
        ///filegroup_name SYSNAME,
        ///physical_name nvarchar(260),
        ///type TINYINT,
        ///size bigint,
        ///space_used bigint,
        ///max_size bigint,
        ///growth bigint,
        ///is_percent_growth bit,
        ///is_read_only BIT,
        ///state TINYINT
        ///)
        ///
        ///DECLARE DBs CURSOR FOR
        ///SELECT name
        ///FROM sys.databases
        ///WHERE state  = 0
        ///AND DATABASEPROPERTYEX(name, &apos;Updateability&apos;) = &apos;READ_WRITE&apos;
        ///
        ///OPEN DBs
        ///FETCH NEXT FROM [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLFiles {
            get {
                return ResourceManager.GetString("SQLFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF OBJECT_ID(&apos;sys.dm_hadr_database_replica_states&apos;) IS NOT NULL
        ///BEGIN
        ///    SELECT database_id,
        ///           group_database_id,
        ///           is_primary_replica,
        ///           synchronization_state,
        ///           synchronization_health,
        ///           is_suspended,
        ///           suspend_reason
        ///    FROM sys.dm_hadr_database_replica_states
        ///    WHERE is_local = 1;
        ///END;.
        /// </summary>
        internal static string SQLHADRDB {
            get {
                return ResourceManager.GetString("SQLHADRDB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WITH t
        ///AS (SELECT rsh.destination_database_name AS database_name,
        ///           rsh.restore_date,
        ///           bs.backup_start_date,
        ///           bmf.physical_device_name AS last_file,
        ///           ROW_NUMBER() OVER (PARTITION BY rsh.destination_database_name
        ///                              ORDER BY rsh.restore_date DESC
        ///                             ) rnum
        ///    FROM msdb.dbo.restorehistory rsh
        ///        INNER JOIN msdb.dbo.backupset bs ON rsh.backup_set_id = bs.backup_set_id
        ///        INNER JOIN msdb.dbo.restoref [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLLogShipping {
            get {
                return ResourceManager.GetString("SQLLogShipping", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @SQL NVARCHAR(MAX);
        ///
        ///WITH cols AS (
        ///	SELECT &apos;softnuma_configuration&apos; AS col,&apos;INT&apos; AS typ
        ///	UNION ALL
        ///	SELECT &apos;sql_memory_model&apos; AS col,&apos;INT&apos; AS typ
        ///	UNION ALL
        ///	SELECT &apos;socket_count&apos; AS col,&apos;INT&apos; AS typ
        ///	UNION ALL
        ///	SELECT &apos;cores_per_socket&apos; AS col,&apos;INT&apos; AS typ
        ///	UNION ALL
        ///	SELECT &apos;numa_node_count&apos; AS col,&apos;INT&apos; AS typ
        ///	UNION ALL
        ///	SELECT &apos;affinity_type&apos; AS col,&apos;INT&apos; AS typ
        ///	UNION ALL
        ///	SELECT &apos;sqlserver_start_time&apos; AS col,&apos;DATETIME&apos; AS typ
        ///	UNION ALL
        ///	SELECT &apos;os_priority_class&apos; AS col,&apos;IN [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLOSInfo {
            get {
                return ResourceManager.GetString("SQLOSInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT CAST(SERVERPROPERTY(&apos;BuildClrVersion&apos;) as nvarchar(128)) as BuildClrVersion ,
        ///CAST(SERVERPROPERTY(&apos;Collation&apos;) as nvarchar(128)) as Collation ,
        ///CAST(SERVERPROPERTY(&apos;CollationID&apos;) as int) as CollationID ,
        ///CAST(SERVERPROPERTY(&apos;ComparisonStyle&apos;) as int) as ComparisonStyle ,
        ///CAST(SERVERPROPERTY(&apos;ComputerNamePhysicalNetBIOS&apos;) as nvarchar(128)) as ComputerNamePhysicalNetBIOS ,
        ///CAST(SERVERPROPERTY(&apos;Edition&apos;) as nvarchar(128)) as Edition ,
        ///CAST(SERVERPROPERTY(&apos;EditionID&apos;) as bigint) as EditionID ,
        ///CAS [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SQLProperties {
            get {
                return ResourceManager.GetString("SQLProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT object_id,
        ///       database_id,
        ///       OBJECT_NAME(object_id, database_id) object_name,
        ///       total_worker_time,
        ///       total_elapsed_time,
        ///       total_logical_reads,
        ///       total_logical_writes,
        ///       total_physical_reads,
        ///       cached_time,
        ///       execution_count,
        ///       GETUTCDATE() AS current_time_utc
        ///FROM sys.dm_exec_procedure_stats
        ///WHERE database_id &lt;&gt; 32767;.
        /// </summary>
        internal static string SQLStoredProcPerformance {
            get {
                return ResourceManager.GetString("SQLStoredProcPerformance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DBCC TRACESTATUS(-1).
        /// </summary>
        internal static string SQLTraceFlags {
            get {
                return ResourceManager.GetString("SQLTraceFlags", resourceCulture);
            }
        }
    }
}
