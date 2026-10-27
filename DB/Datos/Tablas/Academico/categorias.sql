USE lms_db;
GO

IF OBJECT_ID('Academico.categorias', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.categorias (
        id INT IDENTITY(1,1) PRIMARY KEY,
        nombre NVARCHAR(100) NOT NULL UNIQUE,
        descripcion NVARCHAR(255),
        activo BIT DEFAULT 1,
        creado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO