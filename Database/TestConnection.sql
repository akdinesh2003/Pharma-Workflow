-- =============================================
-- Test Database Connection and Data
-- =============================================

USE PharmaWorkflowDB;
GO

-- Check if database exists
SELECT DB_NAME() AS CurrentDatabase;
GO

-- Check all tables
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
GO

-- Check Users
SELECT COUNT(*) AS UserCount FROM Users;
SELECT * FROM Users;
GO

-- Check Main_Transactions
SELECT COUNT(*) AS TransactionCount FROM Main_Transactions WHERE IsDeleted = 0;
SELECT * FROM Main_Transactions WHERE IsDeleted = 0;
GO

-- Check History_Transactions
SELECT COUNT(*) AS HistoryCount FROM History_Transactions;
GO

-- Check if FilePath column exists (should return 0 rows)
SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Main_Transactions' 
AND COLUMN_NAME = 'FilePath';
GO

-- Show all columns in Main_Transactions
SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Main_Transactions'
ORDER BY ORDINAL_POSITION;
GO
