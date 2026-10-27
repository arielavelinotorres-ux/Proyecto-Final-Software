USE lms_db;
GO

IF OBJECT_ID('Academico.cursos', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.cursos (
        id INT IDENTITY(1,1) PRIMARY KEY,
        subcategoria_id INT NOT NULL CONSTRAINT FK_cursos_subcategorias REFERENCES Academico.subcategorias(id),
        docente_id INT NOT NULL CONSTRAINT FK_cursos_docentes REFERENCES Usuarios.docentes(id),
        titulo NVARCHAR(150) NOT NULL,
        descripcion NVARCHAR(MAX),
        codigo_curso NVARCHAR(20) UNIQUE,
        imagen_portada NVARCHAR(255),
        estado NVARCHAR(20) DEFAULT 'Borrador' CHECK (estado IN ('Borrador', 'Publicado', 'Archivado')),
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO