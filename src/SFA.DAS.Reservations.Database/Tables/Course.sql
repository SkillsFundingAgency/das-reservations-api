CREATE TABLE [dbo].[Course]
(
	[CourseId] VARCHAR(20) NOT NULL PRIMARY KEY,
	[Title] VARCHAR(500) NOT NULL,
	[Level] TINYINT NOT NULL,
	[EffectiveTo] DATETIME NULL,
	[ApprenticeshipType] VARCHAR(50) NULL,
	[LearningType] VARCHAR(50) NULL
)
GO;

CREATE NONCLUSTERED INDEX [IX_Course_EffectiveTo_CourseId] ON [dbo].[Course] ([EffectiveTo], [CourseId])
INCLUDE ([Title], [Level], [ApprenticeshipType], [LearningType]) WITH (ONLINE = ON)
GO
