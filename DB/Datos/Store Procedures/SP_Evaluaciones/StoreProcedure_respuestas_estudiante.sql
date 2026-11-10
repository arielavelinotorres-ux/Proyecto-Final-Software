USE lms_db
GO



-- SP para insertar respuesta (cuando el estudiante rinde la evaluación)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_InsertarRespuestaEstudiante
    @estudiante_id INT,
    @pregunta_id INT,
    @opcion_seleccionada_id INT = NULL,
    @respuesta_texto NVARCHAR(MAX) = NULL,
    @es_correcta BIT = NULL,
    @puntaje_obtenido DECIMAL(5,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Evaluaciones.respuestas_estudiante (estudiante_id, pregunta_id, opcion_seleccionada_id, respuesta_texto, es_correcta, puntaje_obtenido, fecha_respuesta)
        OUTPUT INSERTED.id
        VALUES (@estudiante_id, @pregunta_id, @opcion_seleccionada_id, @respuesta_texto, @es_correcta, @puntaje_obtenido, GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al insertar respuesta del estudiante.', 1;
    END CATCH
END;
GO





-- SP para actualizar respuesta (Útil para calificación manual de textos abiertos)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ActualizarRespuestaEstudiante
    @id INT,
    @es_correcta BIT,
    @puntaje_obtenido DECIMAL(5,2)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Evaluaciones.respuestas_estudiante
        SET es_correcta = @es_correcta,
            puntaje_obtenido = @puntaje_obtenido
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: La respuesta a actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al calificar la respuesta.', 1;
    END CATCH
END;
GO





-- SP para eliminar respuesta

CREATE OR ALTER PROCEDURE Evaluaciones.SP_EliminarRespuestaEstudiante
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Evaluaciones.respuestas_estudiante WHERE id = @id;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al eliminar la respuesta.', 1;
    END CATCH
END;
GO





-- SP para listar respuestas por estudiante

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ListarRespuestasEstudiante
    @estudiante_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, estudiante_id, pregunta_id, opcion_seleccionada_id, respuesta_texto, es_correcta, puntaje_obtenido, fecha_respuesta
        FROM Evaluaciones.respuestas_estudiante
        WHERE estudiante_id = @estudiante_id
        ORDER BY fecha_respuesta ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar respuestas.', 1;
    END CATCH
END;
GO


