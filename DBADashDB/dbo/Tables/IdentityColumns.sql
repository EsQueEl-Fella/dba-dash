﻿CREATE TABLE dbo.IdentityColumns(
    InstanceID INT NOT NULL REFERENCES dbo.Instances(InstanceID),
    DatabaseID INT NOT NULL REFERENCES dbo.Databases(DatabaseID),
    SnapshotDate DATETIME2(2) NOT NULL,
    object_id INT NOT NULL,
    object_name NVARCHAR(128)  NULL,
    column_name NVARCHAR(128) NULL,
    last_value BIGINT NULL,
    row_count BIGINT NULL,
    system_type_id TINYINT NOT NULL,
    user_type_id INT NOT NULL,
    max_length SMALLINT NOT NULL,
    increment_value BIGINT NULL,
    seed_value BIGINT NULL,
    min_ident AS CASE max_length WHEN 1 THEN 0 ELSE -POWER(2.,max_length*8-1) END,
    max_ident AS CASE max_length WHEN 1 THEN 255 ELSE POWER(2.,max_length*8-1)-1 END,
    max_rows AS POWER(2.,max_length*8),
    CONSTRAINT PK_IdentityColumns PRIMARY KEY(InstanceID,DatabaseID,object_id)
);
