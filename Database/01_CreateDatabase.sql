-- =============================================
-- Pharma Workflow Management System - Database Creation
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PharmaWorkflowDB')
BEGIN
    ALTER DATABASE PharmaWorkflowDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PharmaWorkflowDB;
END
GO

-- Create database
CREATE DATABASE PharmaWorkflowDB;
GO

USE PharmaWorkflowDB;
GO

PRINT 'Database PharmaWorkflowDB created successfully';
GO
