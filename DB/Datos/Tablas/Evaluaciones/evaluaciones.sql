USE lms_db;
GO

IF OBJECT_ID('Evaluaciones.evaluaciones', 'U') IS NULL
BEGIN
    CREATE TABLE Evaluaciones.evaluaciones (
        id INT IDENTITY(1,1) PRIMARY KEY,
        curso_id INT NOT NULL CONSTRAINT FK_evaluaciones_cursos REFERENCES Academico.cursos(id) ON DELETE CASCADE,
        titulo NVARCHAR(150) NOT NULL,
        descripcion NVARCHAR(MAX) NULL,
        tipo NVARCHAR(30) NOT NULL CHECK (tipo IN ('Tarea', 'Examen', 'Cuestionario', 'Proyecto')),
        puntaje_maximo DECIMAL(5,2) DEFAULT 100.00,
        fecha_inicio DATETIME2 DEFAULT GETDATE(),
        fecha_limite DATETIME2 NULL,
        estado NVARCHAR(20) DEFAULT 'Activo' CHECK (estado IN ('Activo', 'Cerrado', 'Archivado')),
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO


