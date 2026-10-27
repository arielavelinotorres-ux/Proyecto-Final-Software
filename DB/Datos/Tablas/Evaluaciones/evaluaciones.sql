USE lms_db;
GO

IF OBJECT_ID('Evaluaciones.preguntas', 'U') IS NULL
BEGIN
    CREATE TABLE Evaluaciones.preguntas (
        id INT IDENTITY(1,1) PRIMARY KEY,
        evaluacion_id INT NOT NULL CONSTRAINT FK_preguntas_evaluaciones REFERENCES Evaluaciones.evaluaciones(id) ON DELETE CASCADE,
        enunciado NVARCHAR(MAX) NOT NULL,
        tipo_pregunta NVARCHAR(30) NOT NULL CHECK (tipo_pregunta IN ('OpcionMultiple', 'VerdaderoFalso', 'TextoAbierto')),
        puntaje DECIMAL(5,2) DEFAULT 1.00,
        orden INT NOT NULL
    );
END;
GO