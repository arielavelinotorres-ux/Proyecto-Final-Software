USE lms_db;
GO

IF OBJECT_ID('Academico.matriculas', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.matriculas (
        id INT IDENTITY(1,1) PRIMARY KEY,
        estudiante_id INT NOT NULL CONSTRAINT FK_matriculas_estudiantes REFERENCES Usuarios.estudiantes(id),
        curso_id INT NOT NULL CONSTRAINT FK_matriculas_cursos REFERENCES Academico.cursos(id),
        fecha_matricula DATETIME2 DEFAULT GETDATE(),
        estado NVARCHAR(20) DEFAULT 'Activo' CHECK (estado IN ('Activo', 'Completado', 'Cancelado')),
        CONSTRAINT UQ_estudiante_curso UNIQUE (estudiante_id, curso_id)
    );
END;
GO