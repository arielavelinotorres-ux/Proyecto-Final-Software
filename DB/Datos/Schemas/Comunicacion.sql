USE lms_db;
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Comunicacion')
    EXEC('CREATE SCHEMA Comunicacion');
GO