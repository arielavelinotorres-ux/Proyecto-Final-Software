USE lms_db;
GO


IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Usuarios')
    EXEC('CREATE SCHEMA Usuarios');
GO