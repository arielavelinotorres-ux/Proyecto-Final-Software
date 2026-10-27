USE lms_db;
GO

IF OBJECT_ID('Evaluaciones.opciones_pregunta', 'U') IS NULL
BEGIN
    CREATE TABLE Evaluaciones.opciones_pregunta (
        id INT IDENTITY(1,1) PRIMARY KEY,
        pregunta_id INT NOT NULL CONSTRAINT FK_opciones_preguntas REFERENCES Evaluaciones.preguntas(id) ON DELETE CASCADE,
        texto_opcion NVARCHAR(MAX) NOT NULL,
        es_correcta BIT DEFAULT 0,
        orden INT NOT NULL
    );
END;
GO