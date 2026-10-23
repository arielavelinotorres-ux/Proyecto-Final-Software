USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Monitoreo')
    EXEC('CREATE SCHEMA Monitoreo');
GO