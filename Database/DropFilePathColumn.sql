-- =============================================
-- DROP FilePath Column from Main_Transactions
-- Run this to remove FilePath from existing database
-- =============================================

USE PharmaWorkflowDB;
GO

-- Drop the FilePath column
ALTER TABLE Main_Transactions
DROP COLUMN FilePath;
GO

PRINT 'FilePath column removed successfully';
GO

-- Verify the column is gone
SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Main_Transactions'
ORDER BY ORDINAL_POSITION;
GO

-- Show sample data to confirm
SELECT TOP 5 * FROM Main_Transactions;
GO
