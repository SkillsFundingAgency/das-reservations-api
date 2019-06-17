CREATE TABLE [dbo].[UserRuleNotification]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[CourseRuleId] BIGINT NULL,
	[GlobalRuleId] BIGINT NULL,
	[UkPrn] INT NULL,
	[UserId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_UserRuleNotification_Rule] FOREIGN KEY ([CourseRuleId]) REFERENCES [Rule]([Id]),
    CONSTRAINT [FK_UserRuleNotification_GlobalRule] FOREIGN KEY ([GlobalRuleId]) REFERENCES [GlobalRule]([Id])
)
