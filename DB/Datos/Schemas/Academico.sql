USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Academico')
    EXEC('CREATE SCHEMA Academico');
GO