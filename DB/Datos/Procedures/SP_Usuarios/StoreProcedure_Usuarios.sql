USE lms_db
GO

-- SP PARA Insertar Usuario

CREATE OR ALTER PROCEDURE Usuarios.SP_InsertarUsuario
    @rol_id INT,
    @correo NVARCHAR(150),
    @password_hash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Usuarios.usuarios WHERE correo = @correo)
    BEGIN
        ;THROW 50001, 'Error: El correo electrónico ya se encuentra registrado.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Usuarios.usuarios (rol_id, correo, password_hash, activo, ultimo_acceso, creado_en)
        OUTPUT INSERTED.id
        VALUES (@rol_id, @correo, @password_hash, 1, GETDATE(), GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar usuario. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP PARA Actualizar Usuario (Roles, Correo o Contraseña)

CREATE OR ALTER PROCEDURE Usuarios.SP_ActualizarUsuario
    @id INT,
    @rol_id INT,
    @correo NVARCHAR(150),
    @password_hash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Usuarios.usuarios
        SET rol_id = @rol_id,
            correo = @correo,
            password_hash = @password_hash
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El usuario que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar usuario. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP PARA Desactivar Usuario (Borrado lógico)

CREATE OR ALTER PROCEDURE Usuarios.SP_DesactivarUsuario
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Usuarios.usuarios
        SET activo = 0
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El usuario que intenta desactivar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al desactivar usuario. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


-- SP PARA LISTAR USUARIOS

CREATE OR ALTER PROCEDURE Usuarios.SP_ListarUsuarios
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            u.id, 
            u.rol_id, 
            r.nombre AS nombre_rol, 
            u.correo, 
            u.activo, 
            u.ultimo_acceso, 
            u.creado_en
        FROM Usuarios.usuarios u
        INNER JOIN Usuarios.roles r ON u.rol_id = r.id
        ORDER BY u.creado_en DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar usuarios. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO