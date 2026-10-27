USE lms_db;
GO

IF OBJECT_ID('Contenido.secciones', 'U') IS NULL
BEGIN
    CREATE TABLE Contenido.secciones (
        id INT IDENTITY(1,1) PRIMARY KEY,
        curso_id INT NOT NULL CONSTRAINT FK_secciones_cursos REFERENCES Academico.cursos(id) ON DELETE CASCADE,
        titulo NVARCHAR(150) NOT NULL,
        orden INT NOT NULL,
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO