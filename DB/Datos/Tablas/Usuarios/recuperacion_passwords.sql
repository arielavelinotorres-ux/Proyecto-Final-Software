USE lms_db;
GO

IF OBJECT_ID('Usuarios.recuperacion_passwords', 'U') IS NULL
BEGIN
    CREATE TABLE Usuarios.recuperacion_passwords (
        id INT IDENTITY(1,1) PRIMARY KEY,
        usuario_id INT NOT NULL CONSTRAINT FK_recuperacion_usuarios REFERENCES Usuarios.usuarios(id) ON DELETE CASCADE,
        token_hash NVARCHAR(255) NOT NULL,
        expira_en DATETIME2 NOT NULL,
        usado BIT DEFAULT 0
    );
END;
GO