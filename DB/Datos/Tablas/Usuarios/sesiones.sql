USE lms_db;
GO

IF OBJECT_ID('Usuarios.sesiones', 'U') IS NULL
BEGIN
    CREATE TABLE Usuarios.sesiones (
        id INT IDENTITY(1,1) PRIMARY KEY,
        usuario_id INT NOT NULL CONSTRAINT FK_sesiones_usuarios REFERENCES Usuarios.usuarios(id) ON DELETE CASCADE,
        token NVARCHAR(255) NOT NULL,
        ip_origen NVARCHAR(45),
        expira_en DATETIME2 NOT NULL,
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO