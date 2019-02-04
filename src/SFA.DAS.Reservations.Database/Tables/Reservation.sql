CREATE TABLE [dbo].[Reservation]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[IsLevyAccount] TINYINT NOT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
	[StartDate] DATETIME NOT NULL,
	[ExpiryDate] DATETIME NOT NULL, 
    [Status] TINYINT NOT NULL DEFAULT 0,
)
GO;

CREATE NONCLUSTERED INDEX [IDX_Reservation_AccountId] ON [dbo].[Reservation] (AccountId) 
INCLUDE (Id,IsLevyAccount,CreatedDate,ExpiryDate,  StartDate, [Status]) WITH (ONLINE = ON) 
GO;


