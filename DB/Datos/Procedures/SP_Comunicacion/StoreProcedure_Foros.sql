USE lms_db
GO


-- SP para listar Foros Activos por Curso (MODIFICADO)
CREATE OR ALTER PROCEDURE Comunicacion.SP_ListarForos
    @curso_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, curso_id, titulo, descripcion, estado, creado_en 
        FROM Comunicacion.foros 
        WHERE curso_id = @curso_id AND estado != 'Archivado'
        ORDER BY creado_en DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar los foros. Detalle: ' + @DetalleError;
        ;THROW 50003, @MensajeFinal, 1;
    END CATCH
END;
GO



-- SP para la inserccion de foros (Modificado seguimiento de las instrucciones dadas por el Ing. Evelio)

 CREATE OR ALTER PROCEDURE Comunicacion.SP_InsertarForo
    @curso_id INT,
    @titulo NVARCHAR(150),
    @descripcion NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@titulo)) = '' OR @titulo IS NULL
    BEGIN
        ;THROW 50001, 'Error: El título del foro no puede estar vacío.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Comunicacion.foros (curso_id, titulo, descripcion, estado, creado_en)
        OUTPUT INSERTED.id
        VALUES (@curso_id, @titulo, @descripcion, 'Abierto', GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar foro. Detalle: ' + @DetalleError;
        ;THROW 50003, @MensajeFinal, 1;
    END CATCH
END;
GO




-- SP de actualizacion de foro (Modificado)

CREATE OR ALTER PROCEDURE Comunicacion.SP_ActualizarForo
    @id INT,
    @titulo NVARCHAR(150),
    @descripcion NVARCHAR(MAX),
    @estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @estado NOT IN ('Abierto', 'Cerrado', 'Archivado')
    BEGIN
        ;THROW 50004, 'Error: Estado inválido. Solo se permite Abierto, Cerrado o Archivado.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Comunicacion.foros
        SET titulo = @titulo,
            descripcion = @descripcion,
            estado = @estado
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50005, 'Error: El foro que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar foro. Detalle: ' + @DetalleError;
        ;THROW 50003, @MensajeFinal, 1;
    END CATCH
END;
GO



-- SP de desactivar Foro (Usando borrado logico)
CREATE OR ALTER PROCEDURE Comunicacion.SP_DesactivarForo
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Comunicacion.foros
        SET estado = 'Archivado'
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50005, 'Error: El foro que intenta archivar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al desactivar foro. Detalle: ' + @DetalleError;
        ;THROW 50003, @MensajeFinal, 1;
    END CATCH
END;
GO