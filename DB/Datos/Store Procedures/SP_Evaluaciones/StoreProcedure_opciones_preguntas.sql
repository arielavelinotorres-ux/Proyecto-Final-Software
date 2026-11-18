USE lms_db
GO




-- SP PARA INSERTAR OPCION - MODIFICADO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_InsertarOpcionPregunta
    @pregunta_id INT, @texto_opcion NVARCHAR(MAX), @es_correcta BIT, @orden INT
AS
BEGIN
    SET NOCOUNT ON;
    IF LTRIM(RTRIM(@texto_opcion)) = '' OR @texto_opcion IS NULL
        BEGIN ;THROW 50001, 'Error: El texto de la opción no puede estar vacío.', 1; RETURN; END

    BEGIN TRY
        INSERT INTO Evaluaciones.opciones_pregunta (pregunta_id, texto_opcion, es_correcta, orden)
        OUTPUT INSERTED.id
        VALUES (@pregunta_id, @texto_opcion, ISNULL(@es_correcta, 0), @orden);
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar opción. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO



-- SP PARA ACTUALIZAR OPCION - MODIFICADO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ActualizarOpcionPregunta
    @id INT, @texto_opcion NVARCHAR(MAX), @es_correcta BIT, @orden INT
AS
BEGIN
    SET NOCOUNT ON;
    IF LTRIM(RTRIM(@texto_opcion)) = '' OR @texto_opcion IS NULL
        BEGIN ;THROW 50001, 'Error: El texto de la opción no puede estar vacío.', 1; RETURN; END

    BEGIN TRY
        UPDATE Evaluaciones.opciones_pregunta
        SET texto_opcion = @texto_opcion, es_correcta = ISNULL(@es_correcta, 0), orden = @orden
        WHERE id = @id;

        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La opción que intenta actualizar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar opción. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO



-- SP PARA ELIMINAR OPCION - MODIFICADO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_EliminarOpcionPregunta
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Evaluaciones.opciones_pregunta WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La opción que intenta eliminar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar opción. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO




-- SP PARA LISTAR OPCIONES POR PREGUNTA - MODIFICADO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ListarOpcionesPorPregunta
    @pregunta_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT id, pregunta_id, texto_opcion, es_correcta, orden
        FROM Evaluaciones.opciones_pregunta
        WHERE pregunta_id = @pregunta_id
        ORDER BY orden ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar opciones. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO