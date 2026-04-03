-- =============================================
-- Pharma Workflow Management System - Seed Data
-- =============================================

USE PharmaWorkflowDB;
GO

-- Insert Users (Password: Admin@123, Manager@123, User@123)
-- Note: These are hashed passwords - actual implementation will hash in API
INSERT INTO Users (Username, PasswordHash, Role, FullName, Email) VALUES
('admin', 'AQAAAAIAAYagAAAAEKxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9Yx=', 'Admin', 'System Administrator', 'admin@pharma.com'),
('manager', 'AQAAAAIAAYagAAAAEKxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9Yx=', 'Manager', 'QA Manager', 'manager@pharma.com'),
('user', 'AQAAAAIAAYagAAAAEKxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9Yx=', 'User', 'Lab Technician', 'user@pharma.com');
GO

-- Insert Sample Transactions
INSERT INTO Main_Transactions (DrugName, BatchNo, RequestedBy, Status, Comments) VALUES
('Paracetamol 500mg', 'PCM2026B01', 'Lab Technician', 'Approved', 'Initial batch for Q1 2026'),
('Amoxicillin 250mg', 'AMX2026B02', 'Lab Technician', 'Initiated', 'Pending QA approval'),
('Ibuprofen 400mg', 'IBU2026B03', 'Lab Technician', 'Active', 'Production in progress'),
('Aspirin 75mg', 'ASP2026B04', 'Lab Technician', 'Rejected', 'Failed quality check'),
('Metformin 500mg', 'MET2026B05', 'Lab Technician', 'Registered', 'Awaiting approval');
GO

-- Insert History Records
DECLARE @TransId1 INT = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'PCM2026B01');
DECLARE @TransId2 INT = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'AMX2026B02');
DECLARE @TransId3 INT = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'IBU2026B03');
DECLARE @TransId4 INT = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'ASP2026B04');
DECLARE @TransId5 INT = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'MET2026B05');

INSERT INTO History_Transactions (TransactionId, Action, ActionBy, Comments, PreviousStatus, NewStatus) VALUES
(@TransId1, 'Created', 'Lab Technician', 'Request initiated', NULL, 'Initiated'),
(@TransId1, 'Approved', 'QA Manager', 'Quality standards met', 'Initiated', 'Approved'),
(@TransId2, 'Created', 'Lab Technician', 'New batch request', NULL, 'Initiated'),
(@TransId3, 'Created', 'Lab Technician', 'Request initiated', NULL, 'Initiated'),
(@TransId3, 'Approved', 'QA Manager', 'Approved for production', 'Initiated', 'Approved'),
(@TransId3, 'Activated', 'Production Manager', 'Production started', 'Approved', 'Active'),
(@TransId4, 'Created', 'Lab Technician', 'Request initiated', NULL, 'Initiated'),
(@TransId4, 'Rejected', 'QA Manager', 'Quality parameters not met', 'Initiated', 'Rejected'),
(@TransId5, 'Created', 'Lab Technician', 'Request initiated', NULL, 'Initiated'),
(@TransId5, 'Registered', 'System', 'Auto-registered', 'Initiated', 'Registered');
GO

PRINT 'Sample data inserted successfully';
GO

-- Display summary
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Main_Transactions', COUNT(*) FROM Main_Transactions
UNION ALL
SELECT 'History_Transactions', COUNT(*) FROM History_Transactions;
GO


SELECT * FROM Main_Transactions 
WHERE BatchNo = 'ASP2026TEST01';


-- Check History_Transactions table
SELECT * FROM History_Transactions 
WHERE TransactionId = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'ASP2026TEST01');



-- Check if status changed
SELECT Id, DrugName, BatchNo, Status, UpdatedDate 
FROM Main_Transactions 
WHERE BatchNo = 'ASP2026TEST01';







-- Check if approval was recorded in history
SELECT 
    HistoryId,
    Action,
    ActionBy,
    ActionDate,
    PreviousStatus,
    NewStatus,
    Comments
FROM History_Transactions 
WHERE TransactionId = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'ASP2026TEST01')
ORDER BY ActionDate;





-- Check main table
SELECT Id, DrugName, BatchNo, Status 
FROM Main_Transactions 
WHERE BatchNo = 'IBU2026TEST02';

-- Check history
SELECT Action, PreviousStatus, NewStatus, Comments 
FROM History_Transactions 
WHERE TransactionId = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'IBU2026TEST02')
ORDER BY ActionDate;



-- Get the same history from database
SELECT 
    Action,
    ActionBy,
    ActionDate,
    PreviousStatus,
    NewStatus,
    Comments
FROM History_Transactions 
WHERE TransactionId = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'ASP2026TEST01')
ORDER BY ActionDate;




USE PharmaWorkflowDB;

-- Check if transaction was created
SELECT * FROM Main_Transactions 
WHERE BatchNo = 'TEST001';

-- Check if history was created
SELECT * FROM History_Transactions 
WHERE TransactionId = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'TEST001');




-- Check if status changed to Approved
SELECT Id, DrugName, BatchNo, Status, UpdatedDate 
FROM Main_Transactions 
WHERE BatchNo = 'TEST001';

-- Check if approval was recorded in history
SELECT * FROM History_Transactions 
WHERE TransactionId = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'TEST001')
ORDER BY ActionDate DESC;




-- Check rejection
SELECT Id, DrugName, BatchNo, Status 
FROM Main_Transactions 
WHERE BatchNo = 'TEST002';

-- Check history
SELECT Action, ActionBy, Comments, ActionDate 
FROM History_Transactions 
WHERE TransactionId = (SELECT Id FROM Main_Transactions WHERE BatchNo = 'TEST002')
ORDER BY ActionDate;






-- Get complete audit trail
SELECT 
    mt.DrugName,
    mt.BatchNo,
    ht.Action,
    ht.ActionBy,
    ht.PreviousStatus,
    ht.NewStatus,
    ht.Comments,
    ht.ActionDate
FROM History_Transactions ht
JOIN Main_Transactions mt ON ht.TransactionId = mt.Id
WHERE mt.BatchNo = 'TEST001'
ORDER BY ht.ActionDate;





-- Total
SELECT COUNT(*) as Total FROM Main_Transactions WHERE IsDeleted = 0;

-- Approved
SELECT COUNT(*) as Approved FROM Main_Transactions WHERE Status = 'Approved' AND IsDeleted = 0;

-- Pending
SELECT COUNT(*) as Pending FROM Main_Transactions WHERE Status IN ('Initiated', 'Registered') AND IsDeleted = 0;

-- Rejected
SELECT COUNT(*) as Rejected FROM Main_Transactions WHERE Status = 'Rejected' AND IsDeleted = 0;





SELECT * FROM Main_Transactions 
WHERE DrugName LIKE '%Paracetamol%' AND IsDeleted = 0;







USE PharmaWorkflowDB;

-- Summary
PRINT '=== SUMMARY ===';
SELECT 'Total Transactions' as Metric, COUNT(*) as Count FROM Main_Transactions WHERE IsDeleted = 0
UNION ALL
SELECT 'Total History Records', COUNT(*) FROM History_Transactions
UNION ALL
SELECT 'Approved', COUNT(*) FROM Main_Transactions WHERE Status = 'Approved' AND IsDeleted = 0
UNION ALL
SELECT 'Rejected', COUNT(*) FROM Main_Transactions WHERE Status = 'Rejected' AND IsDeleted = 0;

-- Recent Activity
PRINT '=== RECENT ACTIVITY ===';
SELECT TOP 10
    mt.DrugName,
    mt.BatchNo,
    ht.Action,
    ht.ActionBy,
    ht.ActionDate
FROM History_Transactions ht
JOIN Main_Transactions mt ON ht.TransactionId = mt.Id
ORDER BY ht.ActionDate DESC;







USE PharmaWorkflowDB;

-- Table 1: Users
SELECT 'USERS TABLE' as TableName;
SELECT * FROM Users;

-- Table 2: Main Transactions
SELECT 'MAIN TRANSACTIONS TABLE' as TableName;
SELECT * FROM Main_Transactions WHERE IsDeleted = 0;

-- Table 3: History
SELECT 'HISTORY TRANSACTIONS TABLE' as TableName;
SELECT * FROM History_Transactions ORDER BY ActionDate DESC;



-- Check users table
SELECT * FROM Users;

-- Check main transactions table  
SELECT * FROM Main_Transactions ;

-- Check history table
SELECT * FROM History_Transactions;






USE PharmaWorkflowDB;

-- See deleted transactions
SELECT * FROM Main_Transactions WHERE IsDeleted = 1;

-- See who deleted them and when
SELECT 
    mt.Id,
    mt.DrugName,
    mt.BatchNo,
    ht.ActionBy,
    ht.ActionDate,
    ht.Comments
FROM Main_Transactions mt
JOIN History_Transactions ht ON mt.Id = ht.TransactionId
WHERE mt.IsDeleted = 1 AND ht.Action = 'Deleted';



UPDATE Main_Transactions 
SET Status = 'Active' 
WHERE Id = 1;
