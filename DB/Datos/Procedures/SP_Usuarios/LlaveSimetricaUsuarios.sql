USE lms_db;
GO

-- 1. Se va a registrar Nuevo Usuario (Hash SHA2_512) Todo esto lo aprendimos con hect
CREATE OR ALTER PROCEDURE Usuarios.sp_RegistrarUsuario
    @correo NVARCHAR(150),
    @password_plain NVARCHAR(255),
    @rol_id INT
AS
BEGIN
    SET NOCOUNT ON;

    --  se valida si el correo ya existe
    IF EXISTS (SELECT 1 FROM Usuarios.usuarios WHERE correo = @correo)
    BEGIN
        RAISERROR('El correo electrónico ya se encuentra registrado.', 16, 1);
        RETURN;
    END

    INSERT INTO Usuarios.usuarios (
        rol_id, 
        correo, 
        password_hash, 
        activo, 
        creado_en
    )
    VALUES (
        @rol_id, 
        @correo, 
        CONVERT(NVARCHAR(255), HASHBYTES('SHA2_512', @password_plain), 2), 
        1, 
        GETDATE()
    );

    -- Se va a retornar el ID generado para la API en C#
    SELECT SCOPE_IDENTITY() AS usuario_id;
END;
GO

-- se valida el Login del Usuario Con nombre y apellido traídos desde los perfiles.
CREATE OR ALTER PROCEDURE Usuarios.sp_ValidarLogin
    @correo NVARCHAR(150),
    @password_plain NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @hash_guardado NVARCHAR(255);
    DECLARE @hash_ingresado VARBINARY(64);
    DECLARE @usuario_id INT;
    DECLARE @rol_nombre NVARCHAR(50);

    SELECT 
        @usuario_id = u.id,
        @hash_guardado = u.password_hash,
        @rol_nombre = r.nombre
    FROM Usuarios.usuarios u
    INNER JOIN Usuarios.roles r ON u.rol_id = r.id
    WHERE u.correo = @correo AND u.activo = 1;

    -- se hace la condicion si ya hay existencia y estado activo
    IF @usuario_id IS NULL
    BEGIN
        SELECT 0 AS Autenticado, 'Credenciales inválidas o cuenta inactiva' AS Mensaje;
        RETURN;
    END

    -- esto va a generar Hash de la contraseña ingresada
    SET @hash_ingresado = HASHBYTES('SHA2_512', @password_plain);

    -- Aqui se comparara los hashes
    IF CONVERT(NVARCHAR(255), @hash_ingresado, 2) = @hash_guardado
    BEGIN
        -- aqui se actualizar la fecha de último acceso
        UPDATE Usuarios.usuarios 
        SET ultimo_acceso = GETDATE() 
        WHERE id = @usuario_id;

        -- esto te devuelve los datos de autenticación y mas los nombres desde estudiantes o docentes
        SELECT 
            1 AS Autenticado,
            u.id AS usuario_id,
            u.correo,
            @rol_nombre AS rol,
            ISNULL(e.nombre, d.nombre) AS nombre,
            ISNULL(e.apellido, d.apellido) AS apellido
        FROM Usuarios.usuarios u
        LEFT JOIN Usuarios.estudiantes e ON u.id = e.usuario_id
        LEFT JOIN Usuarios.docentes d ON u.id = d.usuario_id
        WHERE u.id = @usuario_id;
    END
    ELSE
    BEGIN
        SELECT 0 AS Autenticado, 'Contraseña incorrecta' AS Mensaje;
    END
END;
GO