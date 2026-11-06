USE lms_db;
GO


-- 1. EN ESTE APARTADO ES LA CREACIÓN DE LLAVE MAESTRA Y CERTIFICADO DE SEGURIDAD COCHONES
IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = '##MS_DatabaseMasterKey##')
BEGIN
    CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Lms_MasterKey_2026!Secure';
END
GO

IF NOT EXISTS (SELECT * FROM sys.certificates WHERE name = 'CertificadoSeguridadLMS')
BEGIN
    CREATE CERTIFICATE CertificadoSeguridadLMS
    WITH SUBJECT = 'Certificado de Cifrado de Datos LMS';
END
GO

IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = 'SymKey_DatosSensibles')
BEGIN
    CREATE SYMMETRIC KEY SymKey_DatosSensibles
    WITH ALGORITHM = AES_256
    ENCRYPTION BY CERTIFICATE CertificadoSeguridadLMS;
END
GO

-- 2.Se Registra Nuevo Usuario (Hash SHA2_512 + Salt)
CREATE OR ALTER PROCEDURE Usuarios.sp_RegistrarUsuario
    @nombre VARCHAR(100),
    @apellido VARCHAR(100),
    @correo VARCHAR(150),
    @password_plain VARCHAR(255),
    @rol_id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Aqui se validad si hay corros duplicados

    IF EXISTS (SELECT 1 FROM Usuarios.usuarios WHERE correo = @correo)
    BEGIN
        RAISERROR('El correo electrónico ya se encuentra registrado.', 16, 1);
        RETURN;
    END

    -- Aquí se genera Salt único y Hash de contraseña
    DECLARE @salt UNIQUEIDENTIFIER = NEWID();
    DECLARE @password_hash VARBINARY(64);
    
    SET @password_hash = HASHBYTES('SHA2_512', CONCAT(@password_plain, CAST(@salt AS VARCHAR(36))));

  
    INSERT INTO Usuarios.usuarios (
        rol_id, 
        nombre, 
        apellido, 
        correo, 
        password_hash, 
        salt, 
        activo, 
        creado_en
    )
    VALUES (
        @rol_id, 
        @nombre, 
        @apellido, 
        @correo, 
        CONVERT(VARCHAR(255), @password_hash, 2), 
        CAST(@salt AS VARCHAR(36)), 
        1, 
        GETDATE()
    );
    -- Agregue este apartado para que retorne el ID generado para que el backend en C# lo reciba y no genere problema.
    SELECT SCOPE_IDENTITY() AS usuario_id;
END;
GO

CREATE OR ALTER PROCEDURE Usuarios.sp_ValidarLogin
    @correo VARCHAR(150),
    @password_plain VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @salt VARCHAR(36);
    DECLARE @hash_guardado VARCHAR(255);
    DECLARE @hash_ingresado VARBINARY(64);
    DECLARE @usuario_id INT;


    SELECT 
        @usuario_id = id,
        @salt = salt,
        @hash_guardado = password_hash
    FROM Usuarios.usuarios
    WHERE correo = @correo AND activo = 1;

    -- Esta condición es para verificar si existe el usuario y si está activo
    IF @usuario_id IS NULL
    BEGIN
        SELECT 0 AS Autenticado, 'Credenciales inválidas o cuenta inactiva' AS Mensaje;
        RETURN;
    END

    -- este set nos ayuda a reconstruir Hash ingresado para que lo lean y investiguen
    SET @hash_ingresado = HASHBYTES('SHA2_512', CONCAT(@password_plain, @salt));

    -- Comparar hashes
    IF CONVERT(VARCHAR(255), @hash_ingresado, 2) = @hash_guardado
    BEGIN
        -- Recuerden que se limpia intentos fallidos previos si lo requiere
        DELETE FROM Usuarios.intentos_fallidos WHERE usuario_id = @usuario_id;

        --Se agrego un retorno de información para que C# cree el Token JWT que usará React que es la michell
        SELECT 
            1 AS Autenticado,
            u.id AS usuario_id,
            u.nombre,
            u.apellido,
            u.correo,
            r.nombre AS rol
        FROM Usuarios.usuarios u
        INNER JOIN Usuarios.roles r ON u.rol_id = r.id
        WHERE u.id = @usuario_id;
    END
    ELSE
    BEGIN
        -- Registramos intento fallido
        INSERT INTO Usuarios.intentos_fallidos (usuario_id, ip_origen, fecha_intento)
        VALUES (@usuario_id, '0.0.0.0', GETDATE());

        SELECT 0 AS Autenticado, 'Contraseña incorrecta' AS Mensaje;
    END
END;
GO

-- 4. Aquí manejamos opcionalmente con Llave Simétrica (AES_256) Esto nos enseño hector
CREATE OR ALTER PROCEDURE Usuarios.sp_GuardarDatoCifrado
    @dato_plano VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_DatosSensibles
    DECRYPTION BY CERTIFICATE CertificadoSeguridadLMS;

    SELECT ENCRYPTBYKEY(KEY_GUID('SymKey_DatosSensibles'), @dato_plano) AS dato_cifrado;

    CLOSE SYMMETRIC KEY SymKey_DatosSensibles;
END;
GO

CREATE OR ALTER PROCEDURE Usuarios.sp_LeerDatoCifrado
    @dato_cifrado VARBINARY(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_DatosSensibles
    DECRYPTION BY CERTIFICATE CertificadoSeguridadLMS;

    SELECT CONVERT(VARCHAR(MAX), DECRYPTBYKEY(@dato_cifrado)) AS dato_desencriptado;

    CLOSE SYMMETRIC KEY SymKey_DatosSensibles;
END;
GO