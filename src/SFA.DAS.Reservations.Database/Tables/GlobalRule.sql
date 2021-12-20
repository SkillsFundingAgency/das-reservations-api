﻿CREATE TABLE [dbo].[GlobalRule]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ActiveFrom] DATETIME NULL,
	[ActiveTo] DATETIME NULL, 
	[Restriction] TINYINT NOT NULL,
	[RuleType] TINYINT NOT NULL, 
    [Exceptions] NVARCHAR(MAX) NULL
)
