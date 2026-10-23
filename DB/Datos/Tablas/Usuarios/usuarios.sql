USE lms_db;
GO

IF NOT EXISTS (
    SELECT * FROM sys.tables t 
    JOIN sys.schemas s ON t.schema_id = s.schema_id 
    WHERE s.name = 'usuarios' AND t.name = 'usuarios'
)
BEGIN
    CREATE TABLE usuarios.usuarios (
        id INT IDENTITY(1,1) PRIMARY KEY,
        rol_id INT NOT NULL CONSTRAINT FK_usuarios_roles REFERENCES usuarios.roles(id),
        correo NVARCHAR(150) NOT NULL UNIQUE,
        password_hash NVARCHAR(255) NOT NULL,
        activo BIT DEFAULT 1,
        ultimo_acceso DATETIME2 DEFAULT GETDATE(),
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO