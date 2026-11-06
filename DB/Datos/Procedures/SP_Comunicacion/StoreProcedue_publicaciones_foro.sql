USE lms_db
GO


-- SP pra insertar Publicación en Foro

CREATE OR ALTER PROCEDURE Comunicacion.SP_InsertarPublicacionForo
    @foro_id INT,
    @usuario_id INT,
    @publicacion_padre_id INT = NULL,
    @contenido NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@contenido)) = '' OR @contenido IS NULL
    BEGIN
        ;THROW 50001, 'Error: El contenido de la publicación no puede estar vacío.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Comunicacion.publicaciones_foro (foro_id, usuario_id, publicacion_padre_id, contenido, creado_en)
        OUTPUT INSERTED.id
        VALUES (@foro_id, @usuario_id, @publicacion_padre_id, @contenido, GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar publicación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

--  SP para actualizar Publicación (Solo se permite editar el contenido)

CREATE OR ALTER PROCEDURE Comunicacion.SP_ActualizarPublicacionForo
    @id INT,
    @contenido NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@contenido)) = '' OR @contenido IS NULL
    BEGIN
        ;THROW 50001, 'Error: El contenido no puede estar vacío.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Comunicacion.publicaciones_foro
        SET contenido = @contenido
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: La publicación que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar publicación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP para liminar Publicación (Borrado físico)

CREATE OR ALTER PROCEDURE Comunicacion.SP_DesactivarPublicacionForo
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Comunicacion.publicaciones_foro
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: La publicación que intenta eliminar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar publicación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP para listar Publicaciones de un Foro Específico

CREATE OR ALTER PROCEDURE Comunicacion.SP_ListarPublicacionesForo
    @foro_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, foro_id, usuario_id, publicacion_padre_id, contenido, creado_en
        FROM Comunicacion.publicaciones_foro
        WHERE foro_id = @foro_id
        ORDER BY creado_en ASC; -- Orden ascendente para leer el hilo desde el inicio
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar publicaciones. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO