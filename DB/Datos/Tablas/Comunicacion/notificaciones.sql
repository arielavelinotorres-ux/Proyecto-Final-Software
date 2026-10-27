USE lms_db;
GO

IF OBJECT_ID('Comunicacion.notificaciones', 'U') IS NULL
BEGIN
    CREATE TABLE Comunicacion.notificaciones (
        id INT IDENTITY(1,1) PRIMARY KEY,
        usuario_id INT NOT NULL CONSTRAINT FK_notificaciones_usuarios REFERENCES Usuarios.usuarios(id) ON DELETE CASCADE,
        titulo NVARCHAR(100) NOT NULL,
        mensaje NVARCHAR(MAX) NOT NULL,
        leido BIT DEFAULT 0,
        fecha_creacion DATETIME2 DEFAULT GETDATE()
    );
END;
GO