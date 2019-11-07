CREATE TABLE [dbo].[ProviderPermission]
(
	[AccountId] BIGINT NOT NULL , 
    [AccountLegalEntityId] BIGINT NOT NULL, 
    [Ukprn] BIGINT NOT NULL, 
    [CanCreateCohort] BIT NOT NULL, 
    PRIMARY KEY ([AccountId],[AccountLegalEntityId],[Ukprn])
)
GO;

CREATE NONCLUSTERED INDEX [IDX_ProviderPermission_AccountId] ON [dbo].[ProviderPermission] (AccountId) 
INCLUDE ([AccountLegalEntityId],[Ukprn],[CanCreateCohort]) WITH (ONLINE = ON) 
GO;

CREATE NONCLUSTERED INDEX [IDX_ProviderPermission_UkprnAccountLegalEntityId] ON [dbo].[ProviderPermission] ([Ukprn], [AccountLegalEntityId]) 
INCLUDE ([AccountId],[CanCreateCohort]) WITH (ONLINE = ON) 
GO;
