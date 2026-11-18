USE lms_db
Go


-- SP para insertar Notificaciones

CREATE OR ALTER PROCEDURE Comunicacion.SP_InsertarNotificacion
    @usuario_id INT, @titulo NVARCHAR(100), @mensaje NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    IF LTRIM(RTRIM(@titulo)) = '' OR @titulo IS NULL
        BEGIN ;THROW 50001, 'Error: El título no puede estar vacío.', 1; RETURN; END
    IF LTRIM(RTRIM(@mensaje)) = '' OR @mensaje IS NULL
        BEGIN ;THROW 50001, 'Error: El mensaje no puede estar vacío.', 1; RETURN; END

    BEGIN TRY
        INSERT INTO Comunicacion.notificaciones (usuario_id, titulo, mensaje, leido, fecha_creacion)
        OUTPUT INSERTED.id
        VALUES (@usuario_id, @titulo, @mensaje, 0, GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar notificación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP para marcar una Notificacion como leida MODIFICADO - (Reemplaza Actualizar)

CREATE OR ALTER PROCEDURE Comunicacion.SP_MarcarNotificacionLeida
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Comunicacion.notificaciones SET leido = 1 WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La notificación no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al marcar notificación como leída. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


-- SP NUEVO - Marca TODAS las Notificaciones como Leídas 

CREATE OR ALTER PROCEDURE Comunicacion.SP_MarcarTodasNotificacionesLeidas
    @usuario_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Comunicacion.notificaciones SET leido = 1 WHERE usuario_id = @usuario_id AND leido = 0;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar notificaciones. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


-- Sp para eliminar Notificación (Borrado físico)

CREATE OR ALTER PROCEDURE Comunicacion.SP_DesactivarNotificacion
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Comunicacion.notificaciones WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La notificación que intenta eliminar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar notificación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


-- SP para listar Notificaciones por Usuario

CREATE OR ALTER PROCEDURE Comunicacion.SP_ListarNotificaciones
    @usuario_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT id, usuario_id, titulo, mensaje, leido, fecha_creacion
        FROM Comunicacion.notificaciones
        WHERE usuario_id = @usuario_id
        ORDER BY fecha_creacion DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar notificaciones. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO