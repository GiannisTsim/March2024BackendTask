USE master;
GO

-- Create the new database if it does not already exist 
IF NOT EXISTS
    (
        SELECT 1
        FROM sys.databases
        WHERE name = N'$(DB_NAME)'
    )
    BEGIN
        CREATE DATABASE $(DB_NAME);
    END
GO