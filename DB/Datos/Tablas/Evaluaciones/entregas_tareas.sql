USE lms_db;
GO

IF OBJECT_ID('Evaluaciones.entregas_tareas', 'U') IS NULL
BEGIN
    CREATE TABLE Evaluaciones.entregas_tareas (
        id INT IDENTITY(1,1) PRIMARY KEY,
        evaluacion_id INT NOT NULL CONSTRAINT FK_entregas_evaluaciones REFERENCES Evaluaciones.evaluaciones(id) ON DELETE CASCADE,
        estudiante_id INT NOT NULL CONSTRAINT FK_entregas_estudiantes REFERENCES Usuarios.estudiantes(id),
        archivo_url NVARCHAR(255),
        contenido_texto NVARCHAR(MAX),
        fecha_entrega DATETIME2 DEFAULT GETDATE(),
        calificacion DECIMAL(5,2) NULL,
        retroalimentacion NVARCHAR(MAX) NULL,
        estado NVARCHAR(20) DEFAULT 'Enviado' CHECK (estado IN ('Enviado', 'Calificado', 'Revisión'))
    );
END;
GO