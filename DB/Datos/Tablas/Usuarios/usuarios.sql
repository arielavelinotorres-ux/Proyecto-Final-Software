USE lms_db;
GO

IF OBJECT_ID('Usuarios.usuarios', 'U') IS NULL
BEGIN
    CREATE TABLE Usuarios.usuarios (
        id INT IDENTITY(1,1) PRIMARY KEY,
        rol_id INT NOT NULL CONSTRAINT FK_usuarios_roles REFERENCES Usuarios.roles(id),
        correo NVARCHAR(150) NOT NULL UNIQUE,
        password_hash NVARCHAR(255) NOT NULL,
        activo BIT DEFAULT 1,
        ultimo_acceso DATETIME2 DEFAULT GETDATE(),
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO