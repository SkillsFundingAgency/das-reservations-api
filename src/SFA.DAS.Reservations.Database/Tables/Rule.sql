CREATE TABLE [dbo].[Rule]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[CourseId] VARCHAR(20) NOT NULL,
	[Restriction] TINYINT NOT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
	[ActiveFrom] DATETIME NOT NULL,
	[ActiveTo] DATETIME NULL, 
    CONSTRAINT [FK_Rule_Apprenticeship] FOREIGN KEY ([CourseId]) REFERENCES [Course]([CourseId]),
)
--TODO Add index for dates