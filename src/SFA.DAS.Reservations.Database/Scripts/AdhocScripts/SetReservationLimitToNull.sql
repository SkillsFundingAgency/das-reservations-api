IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Account'))
BEGIN
	UPDATE [dbo].[Account]
	SET [ReservationLimit] = NULL
    WHERE [ReservationLimit] IS NOT NULL
END