USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Evaluaciones')
    EXEC('CREATE SCHEMA Evaluaciones');
GO