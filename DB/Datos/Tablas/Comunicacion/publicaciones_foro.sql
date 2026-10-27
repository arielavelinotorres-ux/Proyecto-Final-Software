USE lms_db;
GO

IF OBJECT_ID('Comunicacion.publicaciones_foro', 'U') IS NULL
BEGIN
    CREATE TABLE Comunicacion.publicaciones_foro (
        id INT IDENTITY(1,1) PRIMARY KEY,
        foro_id INT NOT NULL CONSTRAINT FK_publicaciones_foros REFERENCES Comunicacion.foros(id) ON DELETE CASCADE,
        usuario_id INT NOT NULL CONSTRAINT FK_publicaciones_usuarios REFERENCES Usuarios.usuarios(id),
        publicacion_padre_id INT NULL CONSTRAINT FK_publicaciones_padre REFERENCES Comunicacion.publicaciones_foro(id),
        contenido NVARCHAR(MAX) NOT NULL,
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO