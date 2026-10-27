USE lms_db;
GO

IF OBJECT_ID('Monitoreo.certificados', 'U') IS NULL
BEGIN
    CREATE TABLE Monitoreo.certificados (
        id INT IDENTITY(1,1) PRIMARY KEY,
        estudiante_id INT NOT NULL CONSTRAINT FK_certificados_estudiantes REFERENCES Usuarios.estudiantes(id),
        curso_id INT NOT NULL CONSTRAINT FK_certificados_cursos REFERENCES Academico.cursos(id),
        codigo_validacion NVARCHAR(50) NOT NULL UNIQUE,
        fecha_emision DATETIME2 DEFAULT GETDATE(),
        url_pdf NVARCHAR(255)
    );
END;
GO