CREATE TABLE [dbo].[GlobalRule]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[ActiveFrom] DATETIME NULL,
	[ActiveTo] DATETIME NULL, 
	[Restriction] TINYINT NOT NULL,
	[RuleType] TINYINT NOT NULL
)
