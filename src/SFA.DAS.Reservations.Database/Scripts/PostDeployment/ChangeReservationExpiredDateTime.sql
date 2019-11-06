IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Reservation'))
BEGIN
	UPDATE [SFA.DAS.Reservations.Database].[dbo].[Reservation]
	SET [ExpiryDate] = DATEADD(second, -1, DATEADD(day, 1, [ExpiryDate]))
	WHERE [ExpiryDate] IS NOT NULL
	AND DATEPART(hour, ExpiryDate) = 0
	AND DATEPART(minute, ExpiryDate) = 0
	AND DATEPART(second, ExpiryDate) = 0
END