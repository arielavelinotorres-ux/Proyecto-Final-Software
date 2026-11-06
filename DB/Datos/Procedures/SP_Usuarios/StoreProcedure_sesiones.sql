USE lms_db
GO


-- SP Para Registrar Nueva Sesión (Login)

CREATE OR ALTER PROCEDURE Usuarios.SP_InsertarSesion
    @usuario_id INT,
    @token NVARCHAR(255),
    @ip_origen NVARCHAR(45),
    @expira_en DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Usuarios.sesiones (usuario_id, token, ip_origen, expira_en, creado_en)
        OUTPUT INSERTED.id
        VALUES (@usuario_id, @token, @ip_origen, @expira_en, GETDATE());
        
        -- Actualizar también la fecha de último acceso en la tabla principal
        UPDATE Usuarios.usuarios SET ultimo_acceso = GETDATE() WHERE id = @usuario_id;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al registrar sesión. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP para eliminar Sesión (Logout)

CREATE OR ALTER PROCEDURE Usuarios.SP_EliminarSesion
    @token NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Usuarios.sesiones
        WHERE token = @token;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al cerrar sesión. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO