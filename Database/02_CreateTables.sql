-- =============================================
-- Pharma Workflow Management System - Table Creation
-- =============================================

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

-- Main_Transactions Table
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
    FilePath NVARCHAR(500),
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

-- Create Indexes for Performance
CREATE INDEX IX_Main_Transactions_Status ON Main_Transactions(Status);
CREATE INDEX IX_Main_Transactions_IsDeleted ON Main_Transactions(IsDeleted);
CREATE INDEX IX_Main_Transactions_CreatedDate ON Main_Transactions(CreatedDate DESC);
CREATE INDEX IX_History_Transactions_TransactionId ON History_Transactions(TransactionId);
CREATE INDEX IX_History_Transactions_ActionDate ON History_Transactions(ActionDate DESC);
GO

PRINT 'Tables created successfully with indexes';
GO
