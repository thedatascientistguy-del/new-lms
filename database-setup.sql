-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LibraryManagementDB')
BEGIN
    CREATE DATABASE LibraryManagementDB;
END
GO

USE LibraryManagementDB;
GO

-- Create Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Create Books Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Books')
BEGIN
    CREATE TABLE Books (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(255) NOT NULL,
        Author NVARCHAR(255) NOT NULL,
        ISBN NVARCHAR(50) NOT NULL,
        PublishedYear INT NOT NULL,
        UserId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
    );
END
GO

-- Create Index on UserId for better query performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_UserId')
BEGIN
    CREATE INDEX IX_Books_UserId ON Books(UserId);
END
GO

PRINT 'Database setup completed successfully!';
