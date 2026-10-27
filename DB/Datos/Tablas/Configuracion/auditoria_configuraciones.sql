USE lms_db;
GO

IF OBJECT_ID('Configuracion.auditoria_configuraciones', 'U') IS NULL
BEGIN
    CREATE TABLE Configuracion.auditoria_configuraciones (
        id INT IDENTITY(1,1) PRIMARY KEY,
        parametro_id INT NOT NULL CONSTRAINT FK_auditoria_parametros REFERENCES Configuracion.parametros_sistema(id) ON DELETE CASCADE,
        usuario_id INT NOT NULL CONSTRAINT FK_auditoria_usuarios REFERENCES Usuarios.usuarios(id),
        valor_anterior NVARCHAR(MAX),
        valor_nuevo NVARCHAR(MAX) NOT NULL,
        fecha_cambio DATETIME2 DEFAULT GETDATE()
    );
END;
GO