USE lms_db;
GO

IF OBJECT_ID('Usuarios.roles', 'U') IS NULL
BEGIN
    CREATE TABLE Usuarios.roles (
        id INT IDENTITY(1,1) PRIMARY KEY,
        nombre NVARCHAR(50) NOT NULL UNIQUE
    );

    -- Insertar roles base del sistema
    INSERT INTO Usuarios.roles (nombre) 
    VALUES ('Administrador'), ('Docente'), ('Estudiante');
END;
GO