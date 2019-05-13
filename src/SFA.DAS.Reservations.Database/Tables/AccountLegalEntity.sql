CREATE TABLE [dbo].[AccountLegalEntity]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[LegalEntityId] BIGINT NOT NULL,
	[Name] VARCHAR(250) not null,
)
