USE lms_db;
GO

IF OBJECT_ID('Usuarios.intentos_fallidos', 'U') IS NULL
BEGIN
    CREATE TABLE Usuarios.intentos_fallidos (
        id INT IDENTITY(1,1) PRIMARY KEY,
        correo NVARCHAR(150) NOT NULL,
        ip_origen NVARCHAR(45) NOT NULL,
        intentos INT DEFAULT 1,
        bloqueado_hasta DATETIME2
    );
END;
GO