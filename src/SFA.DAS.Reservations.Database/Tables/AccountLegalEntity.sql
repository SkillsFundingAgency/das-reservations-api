CREATE TABLE [dbo].[AccountLegalEntity]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[LegalEntityId] BIGINT NOT NULL,
	[AccountLegalEntityId] BIGINT NOT NULL,
	[AccountLegalEntityName] VARCHAR(100) not null,
)
