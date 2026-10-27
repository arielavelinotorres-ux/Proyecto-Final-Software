USE lms_db;
GO

IF OBJECT_ID('Comunicacion.mensajes_privados', 'U') IS NULL
BEGIN
    CREATE TABLE Comunicacion.mensajes_privados (
        id INT IDENTITY(1,1) PRIMARY KEY,
        emisor_id INT NOT NULL CONSTRAINT FK_mensajes_emisor REFERENCES Usuarios.usuarios(id),
        receptor_id INT NOT NULL CONSTRAINT FK_mensajes_receptor REFERENCES Usuarios.usuarios(id),
        asunto NVARCHAR(150),
        contenido NVARCHAR(MAX) NOT NULL,
        leido BIT DEFAULT 0,
        fecha_envio DATETIME2 DEFAULT GETDATE()
    );
END;
GO