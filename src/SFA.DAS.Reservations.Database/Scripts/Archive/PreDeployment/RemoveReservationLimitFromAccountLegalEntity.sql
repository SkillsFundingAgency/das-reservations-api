-- ARCHIVED: No longer run on DACPAC deploy. Retained for auditability.
-- One-time migration: dropped ReservationLimit from AccountLegalEntity and recreated indexes.

IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'AccountLegalEntity' and COLUMN_NAME = 'ReservationLimit')
BEGIN 
	ALTER TABLE [dbo].[AccountLegalEntity] DROP COLUMN [ReservationLimit];
END
GO


DROP INDEX [IDX_AccountLegalEntity_AccountId] ON [dbo].[AccountLegalEntity]
GO
CREATE NONCLUSTERED INDEX [IDX_AccountLegalEntity_AccountId] ON [dbo].[AccountLegalEntity]
(
	[AccountId] ASC
)
INCLUDE([Id],[AccountLegalEntityId],[LegalEntityId],[AccountLegalEntityName],[AgreementSigned],[IsLevy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


DROP INDEX [IDX_AccountLegalEntity_AccountIdLegalEntityId] ON [dbo].[AccountLegalEntity]
GO
CREATE NONCLUSTERED INDEX [IDX_AccountLegalEntity_AccountIdLegalEntityId] ON [dbo].[AccountLegalEntity]
(
	[AccountId] ASC,
	[LegalEntityId] ASC
)
INCLUDE([Id],[AccountLegalEntityId],[AccountLegalEntityName],[AgreementSigned],[IsLevy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


DROP INDEX [IDX_AccountLegalEntity_AccountLegalEntityId] ON [dbo].[AccountLegalEntity]
GO
CREATE NONCLUSTERED INDEX [IDX_AccountLegalEntity_AccountLegalEntityId] ON [dbo].[AccountLegalEntity]
(
	[AccountLegalEntityId] ASC
)
INCLUDE([Id],[AccountId],[LegalEntityId],[AccountLegalEntityName],[AgreementSigned],[IsLevy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
