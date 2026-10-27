USE lms_db;
GO

IF OBJECT_ID('Monitoreo.estudiantes_riesgo', 'U') IS NULL
BEGIN
    CREATE TABLE Monitoreo.estudiantes_riesgo (
        id INT IDENTITY(1,1) PRIMARY KEY,
        estudiante_id INT NOT NULL CONSTRAINT FK_riesgo_estudiantes REFERENCES Usuarios.estudiantes(id),
        curso_id INT NOT NULL CONSTRAINT FK_riesgo_cursos REFERENCES Academico.cursos(id),
        nivel_riesgo NVARCHAR(20) NOT NULL CHECK (nivel_riesgo IN ('Bajo', 'Medio', 'Alto', 'Critico')),
        motivo NVARCHAR(255) NOT NULL,
        detectado_en DATETIME2 DEFAULT GETDATE(),
        atendido BIT DEFAULT 0
    );
END;
GO