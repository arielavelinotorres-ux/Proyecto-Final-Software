USE lms_db
GO


-- SP para Insertar Token de Recuperación

CREATE OR ALTER PROCEDURE Usuarios.SP_InsertarTokenRecuperacion
    @usuario_id INT,
    @token_hash NVARCHAR(255),
    @expira_en DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Usuarios.recuperacion_passwords (usuario_id, token_hash, expira_en, usado)
        OUTPUT INSERTED.id
        VALUES (@usuario_id, @token_hash, @expira_en, 0);
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar token de recuperación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP para Validar Token de Recuperación

CREATE OR ALTER PROCEDURE Usuarios.SP_ValidarTokenRecuperacion
    @token_hash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Devuelve el registro si el token existe, no ha sido usado y no ha expirado
        SELECT id, usuario_id, expira_en, usado
        FROM Usuarios.recuperacion_passwords
        WHERE token_hash = @token_hash 
          AND usado = 0 
          AND expira_en > GETDATE();
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al validar el token de recuperación.', 1;
    END CATCH
END;
GO

-- SP para marcar Token como Usado

CREATE OR ALTER PROCEDURE Usuarios.SP_MarcarTokenUsado
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Usuarios.recuperacion_passwords
        SET usado = 1
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El token que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al marcar token como usado. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO