CREATE TABLE [dbo].[Course]
(
	[CourseId] VARCHAR(20) NOT NULL PRIMARY KEY,
	[Title] VARCHAR(500) NOT NULL,
	[Level] TINYINT NOT NULL,
	[EffectiveTo] DATETIME NULL,
	[ApprenticeshipType] VARCHAR(50) NULL
)
