USE lms_db;
GO

IF OBJECT_ID('Evaluaciones.respuestas_estudiante', 'U') IS NULL
BEGIN
    CREATE TABLE Evaluaciones.respuestas_estudiante (
        id INT IDENTITY(1,1) PRIMARY KEY,
        estudiante_id INT NOT NULL CONSTRAINT FK_respuestas_estudiantes REFERENCES Usuarios.estudiantes(id),
        pregunta_id INT NOT NULL CONSTRAINT FK_respuestas_preguntas REFERENCES Evaluaciones.preguntas(id),
        opcion_seleccionada_id INT NULL CONSTRAINT FK_respuestas_opciones REFERENCES Evaluaciones.opciones_pregunta(id),
        respuesta_texto NVARCHAR(MAX) NULL,
        es_correcta BIT NULL,
        puntaje_obtenido DECIMAL(5,2) NULL,
        fecha_respuesta DATETIME2 DEFAULT GETDATE()
    );
END;
GO