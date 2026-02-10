-- Rename semantics: copy ApprenticeshipType into LearningType for all Course rows (no data loss).
-- Run after DACPAC deploy. ApprenticeshipType column can be removed in a later release.

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Course' AND COLUMN_NAME = 'LearningType')
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Course' AND COLUMN_NAME = 'ApprenticeshipType')
BEGIN
    UPDATE [dbo].[Course]
    SET [LearningType] = [ApprenticeshipType]
    WHERE [ApprenticeshipType] IS NOT NULL;
END
