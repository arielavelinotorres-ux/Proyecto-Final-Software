USE lms_db
GO


-- SP para Guardar Progreso (UPSERT: Fusiona Insertar y Actualizar en uno solo)

CREATE OR ALTER PROCEDURE Monitoreo.SP_GuardarProgresoLeccion
    @estudiante_id INT, @leccion_id INT, @completado BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @fecha_completado DATETIME2 = NULL;

    IF @completado = 1
        BEGIN SET @fecha_completado = GETDATE(); END

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Monitoreo.progreso_lecciones WHERE estudiante_id = @estudiante_id AND leccion_id = @leccion_id)
        BEGIN
            UPDATE Monitoreo.progreso_lecciones
            SET completado = @completado, 
                fecha_completado = CASE WHEN @completado = 1 AND completado = 0 THEN GETDATE() 
                                        WHEN @completado = 0 THEN NULL 
                                        ELSE fecha_completado END
            WHERE estudiante_id = @estudiante_id AND leccion_id = @leccion_id;
        END
        ELSE
        BEGIN
            INSERT INTO Monitoreo.progreso_lecciones (estudiante_id, leccion_id, completado, fecha_completado)
            VALUES (@estudiante_id, @leccion_id, @completado, @fecha_completado);
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al guardar progreso. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO





-- SP para eliminar progreso - MODIFICADO

CREATE OR ALTER PROCEDURE Monitoreo.SP_EliminarProgresoLeccion
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Monitoreo.progreso_lecciones WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: El registro de progreso no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar progreso. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO




-- SP para listar progreso por estudiante - (Con filtro opcional por curso a través de secciones)

CREATE OR ALTER PROCEDURE Monitoreo.SP_ListarProgresoPorEstudiante
    @estudiante_id INT,
    @curso_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            p.id, p.estudiante_id, p.leccion_id, l.titulo AS leccion_titulo,
            s.curso_id, p.completado, p.fecha_completado
        FROM Monitoreo.progreso_lecciones p
        INNER JOIN Contenido.lecciones l ON p.leccion_id = l.id
        INNER JOIN Contenido.secciones s ON l.seccion_id = s.id
        WHERE p.estudiante_id = @estudiante_id
          AND (@curso_id IS NULL OR s.curso_id = @curso_id)
        ORDER BY s.orden ASC, l.orden ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar progreso. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO