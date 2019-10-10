CREATE TABLE [dbo].[ProviderPermission]
(
	[AccountId] BIGINT NOT NULL , 
    [AccountLegalEntityId] BIGINT NOT NULL, 
    [Ukprn] BIGINT NOT NULL, 
    [CanCreateCohort] BIT NOT NULL, 
    PRIMARY KEY ([AccountId],[AccountLegalEntityId],[Ukprn])
)
