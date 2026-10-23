USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Contenido')
    EXEC('CREATE SCHEMA Contenido');
GO