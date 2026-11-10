USE lms_db
GO



-- SP Insertar Pregunta --

CREATE OR ALTER PROCEDURE Evaluaciones.SP_InsertarPregunta
    @evaluacion_id INT,
    @enunciado NVARCHAR(MAX),
    @tipo_pregunta NVARCHAR(30),
    @puntaje DECIMAL(5,2),
    @orden INT
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@enunciado)) = '' OR @enunciado IS NULL
    BEGIN
        ;THROW 50001, 'Error: El enunciado no puede estar vacío.', 1;
        RETURN;
    END

    IF @tipo_pregunta NOT IN ('OpcionMultiple', 'VerdaderoFalso', 'TextoAbierto')
    BEGIN
        ;THROW 50001, 'Error: Tipo de pregunta no válido.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Evaluaciones.preguntas (evaluacion_id, enunciado, tipo_pregunta, puntaje, orden)
        OUTPUT INSERTED.id
        VALUES (@evaluacion_id, @enunciado, @tipo_pregunta, ISNULL(@puntaje, 1.00), @orden);
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar pregunta. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO





-- SP para actualizar Pregunta --

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ActualizarPregunta
    @id INT,
    @enunciado NVARCHAR(MAX),
    @tipo_pregunta NVARCHAR(30),
    @puntaje DECIMAL(5,2),
    @orden INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @tipo_pregunta NOT IN ('OpcionMultiple', 'VerdaderoFalso', 'TextoAbierto')
    BEGIN
        ;THROW 50001, 'Error: Tipo de pregunta no válido.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Evaluaciones.preguntas
        SET enunciado = @enunciado,
            tipo_pregunta = @tipo_pregunta,
            puntaje = @puntaje,
            orden = @orden
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: La pregunta que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al actualizar pregunta.', 1;
    END CATCH
END;
GO





-- SP para eliminar pregunta --

CREATE OR ALTER PROCEDURE Evaluaciones.SP_EliminarPregunta
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Evaluaciones.preguntas WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: La pregunta no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al eliminar pregunta.', 1;
    END CATCH
END;
GO





-- SP para listar Preguntas por Evaluacion


CREATE OR ALTER PROCEDURE Evaluaciones.SP_ListarPreguntasPorEvaluacion
    @evaluacion_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, evaluacion_id, enunciado, tipo_pregunta, puntaje, orden
        FROM Evaluaciones.preguntas
        WHERE evaluacion_id = @evaluacion_id
        ORDER BY orden ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar preguntas.', 1;
    END CATCH
END;
GO