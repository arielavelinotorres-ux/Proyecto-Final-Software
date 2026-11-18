USE lms_db
GO



-- SP para insertar respuesta MODIFICADO (cuando el estudiante rinde la evaluación)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_InsertarRespuestaEstudiante
    @estudiante_id INT, @pregunta_id INT, @opcion_seleccionada_id INT = NULL,
    @respuesta_texto NVARCHAR(MAX) = NULL, @es_correcta BIT = NULL, @puntaje_obtenido DECIMAL(5,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @opcion_seleccionada_id IS NULL AND (LTRIM(RTRIM(@respuesta_texto)) = '' OR @respuesta_texto IS NULL)
        BEGIN ;THROW 50001, 'Error: Debe seleccionar una opción o escribir una respuesta.', 1; RETURN; END

    IF EXISTS (SELECT 1 FROM Evaluaciones.respuestas_estudiante WHERE estudiante_id = @estudiante_id AND pregunta_id = @pregunta_id)
        BEGIN ;THROW 50001, 'Error: Ya has respondido a esta pregunta.', 1; RETURN; END

    BEGIN TRY
        INSERT INTO Evaluaciones.respuestas_estudiante (estudiante_id, pregunta_id, opcion_seleccionada_id, respuesta_texto, es_correcta, puntaje_obtenido, fecha_respuesta)
        OUTPUT INSERTED.id
        VALUES (@estudiante_id, @pregunta_id, @opcion_seleccionada_id, @respuesta_texto, @es_correcta, @puntaje_obtenido, GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar respuesta del estudiante. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO





-- SP para actualizar respuesta (calificación manual)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ActualizarRespuestaEstudiante
    @id INT, @es_correcta BIT, @puntaje_obtenido DECIMAL(5,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Evaluaciones.respuestas_estudiante
        SET es_correcta = @es_correcta, puntaje_obtenido = @puntaje_obtenido
        WHERE id = @id;

        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La respuesta a calificar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al calificar la respuesta. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO




-- SP para eliminar respuesta - MODIFICADO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_EliminarRespuestaEstudiante
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Evaluaciones.respuestas_estudiante WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La respuesta que intenta eliminar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar la respuesta. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO




-- SP para listar respuestas por estudiante - NODIFICADO (Con Filtro opcional por evaluacion)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ListarRespuestasEstudiante
    @estudiante_id INT,
    @evaluacion_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            r.id, r.estudiante_id, r.pregunta_id, r.opcion_seleccionada_id, 
            r.respuesta_texto, r.es_correcta, r.puntaje_obtenido, r.fecha_respuesta
        FROM Evaluaciones.respuestas_estudiante r
        INNER JOIN Evaluaciones.preguntas p ON r.pregunta_id = p.id
        WHERE r.estudiante_id = @estudiante_id 
          AND (@evaluacion_id IS NULL OR p.evaluacion_id = @evaluacion_id)
        ORDER BY r.fecha_respuesta ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar respuestas. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


