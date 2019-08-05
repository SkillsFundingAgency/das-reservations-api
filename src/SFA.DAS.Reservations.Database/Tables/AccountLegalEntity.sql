CREATE TABLE [dbo].[AccountLegalEntity]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[LegalEntityId] BIGINT NOT NULL,
	[AccountLegalEntityId] BIGINT NOT NULL,
	[AccountLegalEntityName] VARCHAR(100) NOT NULL,
	[ReservationLimit] INT NULL,
	[AgreementSigned] BIT NOT NULL DEFAULT 0, 
    [IsLevy] BIT NOT NULL DEFAULT 0, 
    [AgreementType] TINYINT NOT NULL DEFAULT 0
)
GO;

CREATE NONCLUSTERED INDEX [IDX_AccountLegalEntity_AccountLegalEntityId] ON [dbo].[AccountLegalEntity] (AccountLegalEntityId) 
INCLUDE (Id,[AccountId],[LegalEntityId],[AccountLegalEntityName],[ReservationLimit],[AgreementSigned],[IsLevy],[AgreementType]) WITH (ONLINE = ON) 
GO;
