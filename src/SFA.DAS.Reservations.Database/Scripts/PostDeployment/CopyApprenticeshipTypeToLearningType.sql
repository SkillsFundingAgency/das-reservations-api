-- Convert ApprenticeshipType string values to LearningType enum byte values.
-- Run after DACPAC deploy. ApprenticeshipType column can be removed in a later release.
-- Enum mapping: Apprenticeship=0, FoundationApprenticeship=1, ApprenticeshipUnit=2

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Course' AND COLUMN_NAME = 'LearningType')
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Course' AND COLUMN_NAME = 'ApprenticeshipType')
BEGIN
    UPDATE [dbo].[Course]
    SET [LearningType] = CASE 
        WHEN LTRIM(RTRIM([ApprenticeshipType])) = 'Apprenticeship' THEN 0
        WHEN LTRIM(RTRIM([ApprenticeshipType])) = 'FoundationApprenticeship' THEN 1
        WHEN LTRIM(RTRIM([ApprenticeshipType])) = 'ApprenticeshipUnit' THEN 2
        ELSE NULL
    END
    WHERE [ApprenticeshipType] IS NOT NULL;
END
