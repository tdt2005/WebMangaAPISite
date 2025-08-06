CREATE PROCEDURE usp_Login
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ReaderID, Username, Email, Password
    FROM Reader
    WHERE Email = @Email;
END
GO
