CREATE TABLE [dbo].[Account]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	[Name] VARCHAR(500) NOT NULL, 
	[IsLevy] BIT NOT NULL DEFAULT 0,
	[ReservationLimit] INT NULL,   
    CONSTRAINT [AK_Account_Column] UNIQUE ([Id])
)
GO;
