USE lms_db;
GO

IF NOT EXISTS (
    SELECT * FROM sys.tables t 
    JOIN sys.schemas s ON t.schema_id = s.schema_id 
    WHERE s.name = 'Usuarios' AND t.name = 'roles'
)
BEGIN
    CREATE TABLE Usuarios.roles (
        id INT IDENTITY(1,1) PRIMARY KEY,
        nombre NVARCHAR(50) NOT NULL UNIQUE
    );

    -- Datos iniciales del sistema
    INSERT INTO Usuarios.roles (nombre) 
    VALUES ('Administrador'), ('Docente'), ('Estudiante');
END;
GO