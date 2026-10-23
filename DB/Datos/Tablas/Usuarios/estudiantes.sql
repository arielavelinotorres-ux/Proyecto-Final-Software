USE lms_db;
GO

IF OBJECT_ID('Usuarios.estudiantes', 'U') IS NULL
BEGIN
    CREATE TABLE Usuarios.estudiantes (
        id INT IDENTITY(1,1) PRIMARY KEY,
        usuario_id INT NOT NULL UNIQUE CONSTRAINT FK_estudiantes_usuarios REFERENCES Usuarios.usuarios(id) ON DELETE CASCADE,
        codigo_estudiante NVARCHAR(50) UNIQUE,
        nombre NVARCHAR(100) NOT NULL,
        apellido NVARCHAR(100) NOT NULL,
        fecha_nacimiento DATE,
        telefono NVARCHAR(20),
        contacto_emergencia NVARCHAR(150),
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO