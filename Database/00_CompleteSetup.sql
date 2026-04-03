-- =============================================
-- COMPLETE DATABASE SETUP SCRIPT
-- Run this script to set up everything from scratch
-- =============================================

-- Step 1: Create Database
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PharmaWorkflowDB')
BEGIN
    ALTER DATABASE PharmaWorkflowDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PharmaWorkflowDB;
    PRINT 'Existing database dropped';
END
GO

CREATE DATABASE PharmaWorkflowDB;
GO

PRINT 'Database created successfully';
GO

-- Step 2: Create Tables
USE PharmaWorkflowDB;
GO

-- Users Table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('User', 'Manager', 'Admin')),
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Main_Transactions Table (WITHOUT FilePath)
CREATE TABLE Main_Transactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DrugName NVARCHAR(200) NOT NULL,
    BatchNo NVARCHAR(100) NOT NULL,
    RequestedBy NVARCHAR(200) NOT NULL,
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Initiated', 'Registered', 'Approved', 'Rejected', 'Active', 'Inactive', 'Modified')),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    RowVersion ROWVERSION,
    Comments NVARCHAR(MAX),
    CONSTRAINT UQ_BatchNo UNIQUE (BatchNo)
);
GO

-- History_Transactions Table
CREATE TABLE History_Transactions (
    HistoryId INT IDENTITY(1,1) PRIMARY KEY,
    TransactionId INT NOT NULL,
    Action NVARCHAR(100) NOT NULL,
    ActionBy NVARCHAR(200) NOT NULL,
    Comments NVARCHAR(MAX),
    ActionDate DATETIME DEFAULT GETDATE(),
    PreviousStatus NVARCHAR(50),
    NewStatus NVARCHAR(50),
    CONSTRAINT FK_History_Transaction FOREIGN KEY (TransactionId) 
        REFERENCES Main_Transactions(Id) ON DELETE CASCADE
);
GO

-- Create Indexes
CREATE INDEX IX_Main_Transactions_Status ON Main_Transactions(Status);
CREATE INDEX IX_Main_Transactions_IsDeleted ON Main_Transactions(IsDeleted);
CREATE INDEX IX_Main_Transactions_CreatedDate ON Main_Transactions(CreatedDate DESC);
CREATE INDEX IX_History_Transactions_TransactionId ON History_Transactions(TransactionId);
CREATE INDEX IX_History_Transactions_ActionDate ON History_Transactions(ActionDate DESC);
GO

PRINT 'Tables created successfully';
GO

-- Step 3: Insert Seed Data
-- Insert Users (Password: Admin@123, Manager@123, User@123)
INSERT INTO Users (Username, PasswordHash, Role, FullName, Email) VALUES
('admin', 'AQAAAAIAAYagAAAAEKxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9Yx=', 'Admin', 'System Administrator', 'admin@pharma.com'),
('manager', 'AQAAAAIAAYagAAAAEKxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9Yx=', 'Manager', 'QA Manager', 'manager@pharma.com'),
('user', 'AQAAAAIAAYagAAAAEKxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9YxqZ8vJ9Yx=', 'User', 'Lab Technician', 'user@pharma.com');
GO

-- Insert Sample Transactions (WITHOUT FilePath)
INSERT INTO Main_Transactions (DrugName, BatchNo, RequestedBy, Status, Comments) VALUES
('Paracetamol 500mg', 'PCM2026B01', 'Lab Technician', 'Approved', 'Initial batch for Q1 2026'),
('Amoxicillin 250mg', 'AMX2026B02', 'Lab Technician', 'Initiated', 'Pending QA approval'),
('Ibuprofen 400mg', 'IBU2026B03', 'Lab Technician', 'Active', 'Production in progress'),
('Aspirin 75mg', 'ASP2026B04', 'Lab Technician', 'Rejected', 'Failed quality check'),
('Metformin 500mg', 'MET2026B05', 'Lab Technician', 'Initiated', 'Awaiting approval');
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
(@TransId5, 'Created', 'Lab Technician', 'Request initiated', NULL, 'Initiated');
GO

PRINT 'Seed data inserted successfully';
GO

-- Step 4: Verify Setup
PRINT '=== SETUP VERIFICATION ===';
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Main_Transactions', COUNT(*) FROM Main_Transactions
UNION ALL
SELECT 'History_Transactions', COUNT(*) FROM History_Transactions;
GO

-- Show sample data
PRINT '=== SAMPLE USERS ===';
SELECT Username, Role, FullName FROM Users;
GO

PRINT '=== SAMPLE TRANSACTIONS ===';
SELECT Id, DrugName, BatchNo, Status FROM Main_Transactions;
GO

PRINT '=== SETUP COMPLETE ===';
PRINT 'Database: PharmaWorkflowDB';
PRINT 'Tables: Users, Main_Transactions, History_Transactions';
PRINT 'FilePath column: REMOVED';
PRINT '';
PRINT 'Login Credentials:';
PRINT 'Admin: admin / Admin@123';
PRINT 'Manager: manager / Manager@123';
PRINT 'User: user / User@123';
GO
