USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Configuracion')
    EXEC('CREATE SCHEMA Configuracion');
GO