USE lms_db;
GO

IF OBJECT_ID('Configuracion.parametros_sistema', 'U') IS NULL
BEGIN
    CREATE TABLE Configuracion.parametros_sistema (
        id INT IDENTITY(1,1) PRIMARY KEY,
        clave NVARCHAR(100) NOT NULL UNIQUE,
        valor NVARCHAR(MAX) NOT NULL,
        descripcion NVARCHAR(255),
        tipo_dato NVARCHAR(20) DEFAULT 'Texto' CHECK (tipo_dato IN ('Texto', 'Numero', 'Booleano', 'JSON')),
        actualizado_en DATETIME2 DEFAULT GETDATE()
    );
END;
GO