CREATE TABLE [dbo].[UserRuleNotification]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[CourseRuleId] BIGINT NULL,
	[GlobalRuleId] BIGINT NULL,
	[UkPrn] INT NULL,
	[UserId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_UserRuleNotification_Rule] FOREIGN KEY ([CourseRuleId]) REFERENCES [Rule]([Id]),
    CONSTRAINT [FK_UserRuleNotification_GlobalRule] FOREIGN KEY ([GlobalRuleId]) REFERENCES [GlobalRule]([Id])
)
