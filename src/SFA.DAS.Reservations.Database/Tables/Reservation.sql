CREATE TABLE [dbo].[Reservation]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[IsLevyAccount] TINYINT NOT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
	[StartDate] DATETIME NOT NULL,
	[ExpiryDate] DATETIME NOT NULL, 
    [Status] TINYINT NOT NULL DEFAULT 0, 
    [CourseId] VARCHAR(20) NULL, 
    CONSTRAINT [FK_Reservation_Course] FOREIGN KEY (CourseId) REFERENCES [Course]([CourseId]),
)
GO;

CREATE NONCLUSTERED INDEX [IDX_Reservation_AccountId] ON [dbo].[Reservation] (AccountId) 
INCLUDE (Id,IsLevyAccount,CreatedDate,ExpiryDate,  StartDate, [Status], CourseId) WITH (ONLINE = ON) 
GO;


