CREATE PROCEDURE usp_SignUp
    @Username NVARCHAR(100),
    @Email NVARCHAR(100),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM Reader WHERE Email = @Email OR Username = @Username)
    BEGIN
        RAISERROR('Username or email already exists.', 16, 1);
        RETURN;
    END

    INSERT INTO Reader (Username, Email, Password)
    VALUES (@Username, @Email, @Password);
END
GO
