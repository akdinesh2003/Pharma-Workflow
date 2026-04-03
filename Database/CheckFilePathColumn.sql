-- =============================================
-- Check if FilePath column exists in Main_Transactions table
-- =============================================

USE PharmaWorkflowDB;
GO

-- Check if FilePath column exists
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Main_Transactions' 
AND COLUMN_NAME = 'FilePath';

-- If the above query returns no rows, the column has been deleted
-- If it returns 1 row, the column still exists

GO

-- Show all columns in Main_Transactions table
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Main_Transactions'
ORDER BY ORDINAL_POSITION;

GO
