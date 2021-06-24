CREATE TABLE [dbo].[AccountLegalEntity]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[LegalEntityId] BIGINT NOT NULL,
	[AccountLegalEntityId] BIGINT NOT NULL,
	[AccountLegalEntityName] VARCHAR(100) NOT NULL,
	[AgreementSigned] BIT NOT NULL DEFAULT 0, 
    [IsLevy] BIT NOT NULL DEFAULT 0   
    CONSTRAINT [AK_AccountLegalEntity_Column] UNIQUE ([AccountLegalEntityId])
)
GO;

CREATE NONCLUSTERED INDEX [IDX_AccountLegalEntity_AccountLegalEntityId] ON [dbo].[AccountLegalEntity] (AccountLegalEntityId) 
INCLUDE (Id,[AccountId],[LegalEntityId],[AccountLegalEntityName],[AgreementSigned],[IsLevy]) WITH (ONLINE = ON) 
GO;

CREATE NONCLUSTERED INDEX [IDX_AccountLegalEntity_AccountId] ON [dbo].[AccountLegalEntity] (AccountId) 
INCLUDE (Id,[AccountLegalEntityId],[LegalEntityId],[AccountLegalEntityName],[AgreementSigned],[IsLevy]) WITH (ONLINE = ON) 
GO;

CREATE NONCLUSTERED INDEX [IDX_AccountLegalEntity_AccountIdLegalEntityId] ON [dbo].[AccountLegalEntity] (AccountId, LegalEntityId) 
INCLUDE (Id,[AccountLegalEntityId],[AccountLegalEntityName],[AgreementSigned],[IsLevy]) WITH (ONLINE = ON) 
GO;