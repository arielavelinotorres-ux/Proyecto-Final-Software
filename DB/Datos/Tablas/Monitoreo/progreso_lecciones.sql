USE lms_db;
GO

IF OBJECT_ID('Monitoreo.progreso_lecciones', 'U') IS NULL
BEGIN
    CREATE TABLE Monitoreo.progreso_lecciones (
        id INT IDENTITY(1,1) PRIMARY KEY,
        estudiante_id INT NOT NULL CONSTRAINT FK_progreso_estudiantes REFERENCES Usuarios.estudiantes(id),
        leccion_id INT NOT NULL CONSTRAINT FK_progreso_lecciones REFERENCES Contenido.lecciones(id),
        completado BIT DEFAULT 0,
        fecha_completado DATETIME2 NULL,
        CONSTRAINT UQ_estudiante_leccion UNIQUE (estudiante_id, leccion_id)
    );
END;
GO