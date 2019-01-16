CREATE TABLE [dbo].[Reservation]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[AccountId] BIGINT NOT NULL,
	[IsLevyAccount] TINYINT NOT NULL,
	[ApprenticeId] BIGINT NULL,
	[VacancyId] BIGINT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
	[StartDate] DATETIME NOT NULL,
	[ExpiryDate] DATETIME NOT NULL,
)
GO;

CREATE NONCLUSTERED INDEX [IDX_Reservation_AccountId] ON [dbo].[Reservation] (AccountId) 
INCLUDE (Id,IsLevyAccount,ApprenticeId,CreatedDate,ExpiryDate, VacancyId, StartDate) WITH (ONLINE = ON) 
GO;

CREATE NONCLUSTERED INDEX [IDX_Reservation_AccountId_ApprenticeId] ON [dbo].[Reservation] (AccountId, ApprenticeId) 
INCLUDE (Id,IsLevyAccount,CreatedDate,ExpiryDate, VacancyId,StartDate) 
WHERE(ApprenticeId IS NOT NULL)
WITH (ONLINE = ON) 
GO;


CREATE NONCLUSTERED INDEX [IDX_Reservation_AccountId_VacancyId] ON [dbo].[Reservation] (AccountId, VacancyId) 
INCLUDE (Id,IsLevyAccount,CreatedDate,ExpiryDate,ApprenticeId,StartDate) 
WHERE(VacancyId is not null)
WITH (ONLINE = ON) 
GO;

