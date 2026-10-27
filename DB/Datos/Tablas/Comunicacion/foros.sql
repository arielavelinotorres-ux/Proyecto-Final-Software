USE lms_db;
GO

IF OBJECT_ID('Comunicacion.foros', 'U') IS NULL
BEGIN
    CREATE TABLE Comunicacion.foros (
        id INT IDENTITY(1,1) PRIMARY KEY,
        curso_id INT NOT NULL CONSTRAINT FK_foros_cursos REFERENCES Academico.cursos(id) ON DELETE CASCADE,
        titulo NVARCHAR(150) NOT NULL,
        descripcion NVARCHAR(MAX),
        estado NVARCHAR(20) DEFAULT 'Abierto' CHECK (estado IN ('Abierto', 'Cerrado', 'Archivado')),
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO