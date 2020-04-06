﻿CREATE VIEW [dbo].[InstanceInfo]
AS
SELECT I.InstanceID,
	I.Instance,
	I.ConnectionID,
    I.MachineName,
	I.ComputerNamePhysicalNetBIOS,
	I.ServerName,
	I.InstanceName,
	I.ActivePowerPlanGUID,
    I.ActivePowerPlan,
    I.ProcessorNameString,
    I.SystemManufacturer,
    I.SystemProductName,
	I.physical_memory_kb,
	I.physical_memory_kb/POWER(1024.0,2) PhysicalMemoryGB,
	CAST(maxmem.value AS BIGINT) BufferPoolMB, 
	CAST(maxmem.value AS BIGINT) / (I.physical_memory_kb/1024.0) PctMemoryAllocatedToBufferPool,
	((I.physical_memory_kb/1024)-CAST(maxmem.value AS BIGINT)) / 1024.0 AS MemoryNotAllocatedToBufferPoolGB,
    I.sql_memory_model,	  
   	CASE I.sql_memory_model WHEN 1 THEN 'CONVENTIONAL' WHEN 2 THEN 'LOCK_PAGES' WHEN 3 THEN 'LARGE_PAGES' ELSE NULL END AS sql_memory_model_desc,
	I.OfflineSchedulers,
    I.affinity_type,
	CASE I.affinity_type WHEN 1 THEN 'MANUAL' WHEN 2 THEN 'AUTO' ELSE NULL END AS affinity_type_desc,
    I.cores_per_socket,
	I.socket_count,
	I.cores_per_socket*I.socket_count AS cpu_core_count,
	I.cpu_count/I.hyperthread_ratio AS physical_cpu_count,
    I.cpu_count,
    I.hyperthread_ratio,
	I.numa_node_count,
    I.softnuma_configuration,
	CASE I.softnuma_configuration WHEN 0 THEN 'OFF' WHEN 1 THEN 'ON' WHEN 2 THEN 'MANUAL' ELSE NULL END AS softnuma_configuration_desc,
    I.ms_ticks,
	I.ms_ticks/60000 host_up_time_mins,
	DATEADD(s,-I.ms_ticks/1000,os.SnapshotDate) AS host_start_time,
	I.sqlserver_start_time,
    I.os_priority_class,
	CASE I.os_priority_class WHEN 32 THEN 'NORMAL' WHEN 128 THEN 'HIGH' ELSE NULL END AS os_priority_class_desc,
    I.max_workers_count,
    I.scheduler_count,
    I.BuildClrVersion,
	I.LicenseType,
    I.NumLicenses,
    I.ProductBuild,
    I.ProductBuildType,
    I.ProductLevel,
    I.ProductMajorVersion,
    I.ProductUpdateLevel,
    I.ProductUpdateReference,
    I.ProductVersion,
	I.Edition,
    I.EditionID,
    I.EngineEdition,
	I.ResourceLastUpdateDateTime,
    I.ResourceVersion,
	CASE WHEN I.ProductVersion LIKE '9.%' THEN 'SQL 2005' 
			WHEN I.ProductVersion LIKE '10.0%' THEN 'SQL 2008' 
			WHEN I.ProductVersion LIKE '10.5%' THEN 'SQL 2008 R2'
			WHEN I.ProductVersion LIKE '11.%' THEN 'SQL 2012'
			WHEN I.ProductVersion LIKE '12.%' THEN 'SQL 2014'
			WHEN I.ProductVersion LIKE '13.%' THEN 'SQL 2016'
			WHEN I.ProductVersion LIKE '14.%' THEN 'SQL 2017'
			WHEN I.ProductVersion LIKE '15.%' THEN 'SQL 2019'
			ELSE I.ProductVersion END + ' ' + ISNULL(I.Edition + ' ','') + 
						ISNULL(I.ProductLevel + ' ','') + ISNULL(I.ProductUpdateLevel,'') AS SQLVersion,
    I.Collation,
    I.CollationID,
    I.ComparisonStyle,
	I.LCID,
    I.SqlCharSet,
    I.SqlCharSetName,
    I.SqlSortOrder,
    I.SqlSortOrderName,
    I.FileStreamConfiguredLevel,
    I.FileStreamEffectiveLevel,
    I.FileStreamShareName,
    I.HadrManagerStatus,
    I.InstanceDefaultDataPath,
    I.InstanceDefaultLogPath,
    I.IsAdvancedAnalyticsInstalled,
    I.IsClustered,
    I.IsFullTextInstalled,
    I.IsHadrEnabled,
    I.IsIntegratedSecurityOnly,
    I.IsLocalDB,
    I.IsPolybaseInstalled,
    I.IsXTPSupported,
    I.IsAgentRunning,
    I.InstantFileInitializationEnabled,
    I.AgentHostName,
	I.WindowsCaption,
	I.WindowsRelease,
	I.WindowsSKU,
	I.IsActive,
	DATEDIFF(mi,DATEADD(mi,I.UtcOffSet,I.sqlserver_start_time),os.SnapshotDate) AS sqlserver_uptime
FROM dbo.Instances I
JOIN dbo.SysConfig maxmem ON maxmem.InstanceID = I.InstanceID AND maxmem.configuration_id=1544 
LEFT JOIN dbo.CollectionDates os ON os.InstanceID = I.InstanceID AND os.Reference='OSInfo'