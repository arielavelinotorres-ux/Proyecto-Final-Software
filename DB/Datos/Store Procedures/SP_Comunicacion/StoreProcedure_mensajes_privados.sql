USE lms_db
GO  

-- SP para insertar los mensajes privados (MODIFICADO)

CREATE OR ALTER PROCEDURE Comunicacion.SP_InsertarMensajePrivado
    @emisor_id INT, @receptor_id INT, @asunto NVARCHAR(150), @contenido NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    IF @emisor_id = @receptor_id
        BEGIN ;THROW 50001, 'Error: No puedes enviarte un mensaje a ti mismo.', 1; RETURN; END
    IF LTRIM(RTRIM(@contenido)) = '' OR @contenido IS NULL
        BEGIN ;THROW 50001, 'Error: El contenido del mensaje no puede estar vacío.', 1; RETURN; END

    BEGIN TRY
        INSERT INTO Comunicacion.mensajes_privados (emisor_id, receptor_id, asunto, contenido, leido, fecha_envio)
        OUTPUT INSERTED.id
        VALUES (@emisor_id, @receptor_id, @asunto, @contenido, 0, GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar mensaje. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


--SP para actualizar los mensajes privados (MODIFICDO) - SP Cambiado a Marcar mensaje como leido 


CREATE OR ALTER PROCEDURE Comunicacion.SP_MarcarMensajeComoLeido
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Comunicacion.mensajes_privados SET leido = 1 WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: El mensaje no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar estado de lectura. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


--SP para Eliminar los mensajes (MODIFICADO - Borrado Fisico)

CREATE OR ALTER PROCEDURE Comunicacion.SP_DesactivarMensajePrivado
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Comunicacion.mensajes_privados WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: El mensaje que intenta eliminar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar mensaje. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


-- SP antes para listar ahora es bandeja de entrada (MODIFICADO - Mensajes recibidos con nombre del emisor)

CREATE OR ALTER PROCEDURE Comunicacion.SP_ListarMensajesPrivados
    @receptor_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            m.id, m.emisor_id, 
            COALESCE(d.nombre + ' ' + d.apellido, e.nombre + ' ' + e.apellido, u.correo) AS emisor_nombre,
            m.receptor_id, m.asunto, m.contenido, m.leido, m.fecha_envio
        FROM Comunicacion.mensajes_privados m
        INNER JOIN Usuarios.usuarios u ON m.emisor_id = u.id
        LEFT JOIN Usuarios.docentes d ON u.id = d.usuario_id
        LEFT JOIN Usuarios.estudiantes e ON u.id = e.usuario_id
        WHERE m.receptor_id = @receptor_id
        ORDER BY m.fecha_envio DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar bandeja de entrada. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO



-- Nuevo SP - Bandeja de Salida (Mensajes Enviados con nombre del receptor)

CREATE OR ALTER PROCEDURE Comunicacion.SP_ListarMensajesEnviados
    @emisor_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            m.id, m.emisor_id, m.receptor_id, 
            COALESCE(d.nombre + ' ' + d.apellido, e.nombre + ' ' + e.apellido, u.correo) AS receptor_nombre,
            m.asunto, m.contenido, m.leido, m.fecha_envio
        FROM Comunicacion.mensajes_privados m
        INNER JOIN Usuarios.usuarios u ON m.receptor_id = u.id
        LEFT JOIN Usuarios.docentes d ON u.id = d.usuario_id
        LEFT JOIN Usuarios.estudiantes e ON u.id = e.usuario_id
        WHERE m.emisor_id = @emisor_id
        ORDER BY m.fecha_envio DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar bandeja de salida. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO