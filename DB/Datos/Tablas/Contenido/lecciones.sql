USE lms_db;
GO

IF OBJECT_ID('Contenido.lecciones', 'U') IS NULL
BEGIN
    CREATE TABLE Contenido.lecciones (
        id INT IDENTITY(1,1) PRIMARY KEY,
        seccion_id INT NOT NULL CONSTRAINT FK_lecciones_secciones REFERENCES Contenido.secciones(id) ON DELETE CASCADE,
        titulo NVARCHAR(150) NOT NULL,
        contenido_texto NVARCHAR(MAX),
        url_video NVARCHAR(255),
        duracion_minutos INT DEFAULT 0,
        orden INT NOT NULL,
        es_gratuita BIT DEFAULT 0,
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO