USE lms_db;
GO

IF OBJECT_ID('Usuarios.docentes', 'U') IS NULL
BEGIN
    CREATE TABLE Usuarios.docentes (
        id INT IDENTITY(1,1) PRIMARY KEY,
        usuario_id INT NOT NULL UNIQUE CONSTRAINT FK_docentes_usuarios REFERENCES Usuarios.usuarios(id) ON DELETE CASCADE,
        codigo_docente NVARCHAR(50) UNIQUE,
        nombre NVARCHAR(100) NOT NULL,
        apellido NVARCHAR(100) NOT NULL,
        especialidad NVARCHAR(150),
        telefono NVARCHAR(20),
        biografia NVARCHAR(MAX),
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO