USE lms_db
GO  

-- SP para insertar los mensajes privados

CREATE OR ALTER PROCEDURE Comunicacion.SP_InsertarMensajePrivado
    @emisor_id INT,
    @receptor_id INT,
    @asunto NVARCHAR(150),
    @contenido NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    -- Evita que el usuario se envie mensaje a si mismo 
    IF @emisor_id = @receptor_id
    BEGIN
        ;THROW 50001, 'Error: No puedes enviarte un mensaje a ti mismo.', 1;
        RETURN;
    END

    -- evita que el mensaje este vacio 
    IF LTRIM(RTRIM(@contenido)) = '' OR @contenido IS NULL
    BEGIN
        ;THROW 50002, 'Error: El contenido del mensaje no puede estar vacío.', 1;
        RETURN;
    END

    -- Límitacion de caracteres
    IF LEN(@contenido) > 200
    BEGIN
        ;THROW 50003, 'Error: El mensaje supera los 200 caracteres permitidos.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Comunicacion.mensajes_privados (emisor_id, receptor_id, asunto, contenido, leido, fecha_envio, activo)
        OUTPUT INSERTED.id
        VALUES (@emisor_id, @receptor_id, @asunto, @contenido, 0, GETDATE(), 1);
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar mensaje. Detalle: ' + @DetalleError;
        ;THROW 50004, @MensajeFinal, 1;
    END CATCH
END;
GO



--SP para actualizar los mensajes privados


CREATE OR ALTER PROCEDURE Comunicacion.SP_ActualizarMensajePrivado
    @id INT,
    @asunto NVARCHAR(150),
    @contenido NVARCHAR(200),
    @leido BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@contenido)) = '' OR @contenido IS NULL
    BEGIN
        ;THROW 50002, 'Error: El contenido del mensaje no puede estar vacío.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Comunicacion.mensajes_privados
        SET asunto = @asunto,
            contenido = @contenido,
            leido = @leido
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50005, 'Error: El mensaje que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar mensaje. Detalle: ' + @DetalleError;
        ;THROW 50004, @MensajeFinal, 1;
    END CATCH
END;
GO



--SP para desactivar los mensajes 

CREATE OR ALTER PROCEDURE Comunicacion.SP_DesactivarMensajePrivado
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Comunicacion.mensajes_privados
        SET activo = 0
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50005, 'Error: El mensaje que intenta eliminar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar mensaje. Detalle: ' + @DetalleError;
        ;THROW 50004, @MensajeFinal, 1;
    END CATCH
END;
GO


-- SP para listar los mensajes se puso en orden del mas reciente al mas antiguo 

CREATE OR ALTER PROCEDURE Comunicacion.SP_ListarMensajesPrivados
    @receptor_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, emisor_id, receptor_id, asunto, contenido, leido, fecha_envio
        FROM Comunicacion.mensajes_privados
        WHERE receptor_id = @receptor_id AND activo = 1
        ORDER BY fecha_envio DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar mensajes. Detalle: ' + @DetalleError;
        ;THROW 50004, @MensajeFinal, 1;
    END CATCH
END;
GO