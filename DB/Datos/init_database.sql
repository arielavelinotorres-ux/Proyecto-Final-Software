USE master;
GO

-- Eliminar la base de datos si existe para limpiar cualquier error anterior
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'lms_db')
BEGIN
    ALTER DATABASE lms_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE lms_db;
END;
GO

-- Crear la base de datos limpia
CREATE DATABASE lms_db;
GO

USE lms_db;
GO