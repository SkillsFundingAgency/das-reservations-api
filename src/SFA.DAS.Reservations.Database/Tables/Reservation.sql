﻿CREATE TABLE [dbo].[Reservation]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[AccountId] BIGINT NOT NULL,
	[IsLevyAccount] TINYINT NOT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
	[StartDate] DATETIME NULL,
	[ExpiryDate] DATETIME NULL, 
    [Status] TINYINT NOT NULL DEFAULT 0, 
    [CourseId] VARCHAR(20) NULL, 
	[AccountLegalEntityId] BIGINT NULL,
	[ProviderId] INT NULL,
    [AccountLegalEntityName] VARCHAR(100) NULL, 
	[TransferSenderAccountId] BIGINT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
	[ClonedReservationId] UNIQUEIDENTIFIER NULL,
    [ConfirmedDate] DATETIME NULL, 
    [CohortId] BIGINT NULL, 
    [DraftApprenticeshipId] BIGINT NULL, 
    CONSTRAINT [FK_Reservation_Course] FOREIGN KEY (CourseId) REFERENCES [Course]([CourseId]),
)
GO;

CREATE NONCLUSTERED INDEX [IDX_Reservation_AccountId] ON [dbo].[Reservation] (AccountId) 
INCLUDE (Id,IsLevyAccount,CreatedDate,ExpiryDate,  StartDate, [Status], CourseId,[AccountLegalEntityId],[ProviderId], AccountLegalEntityName,[TransferSenderAccountId], [UserId]) WITH (ONLINE = ON) 
GO;

CREATE NONCLUSTERED INDEX [IDX_Reservation_ProviderId] ON [dbo].[Reservation] (ProviderId) 
INCLUDE (Id,IsLevyAccount,CreatedDate,ExpiryDate,  StartDate, [Status], CourseId,[AccountLegalEntityId],AccountId, AccountLegalEntityName,[TransferSenderAccountId], [UserId]) WITH (ONLINE = ON) 
GO;

CREATE NONCLUSTERED INDEX [IDX_Reservation_AccountLegalEntityIsLevyAccountStatus] ON [dbo].[Reservation] ([AccountLegalEntityId], [IsLevyAccount], [Status]) WITH (ONLINE = ON)
GO;