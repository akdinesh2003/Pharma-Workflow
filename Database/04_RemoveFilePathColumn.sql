-- =============================================
-- Remove FilePath Column from Main_Transactions
-- Run this script if you already have the database created
-- =============================================

USE PharmaWorkflowDB;
GO

-- Check if FilePath column exists before dropping
IF EXISTS (
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Main_Transactions' 
    AND COLUMN_NAME = 'FilePath'
)
BEGIN
    ALTER TABLE Main_Transactions
    DROP COLUMN FilePath;
    
    PRINT 'FilePath column removed successfully from Main_Transactions table';
END
ELSE
BEGIN
    PRINT 'FilePath column does not exist in Main_Transactions table';
END
GO
