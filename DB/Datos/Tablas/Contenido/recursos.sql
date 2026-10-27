USE lms_db;
GO

IF OBJECT_ID('Contenido.recursos', 'U') IS NULL
BEGIN
    CREATE TABLE Contenido.recursos (
        id INT IDENTITY(1,1) PRIMARY KEY,
        leccion_id INT NOT NULL CONSTRAINT FK_recursos_lecciones REFERENCES Contenido.lecciones(id) ON DELETE CASCADE,
        nombre NVARCHAR(150) NOT NULL,
        tipo_archivo NVARCHAR(50),
        url_recurso NVARCHAR(255) NOT NULL,
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO