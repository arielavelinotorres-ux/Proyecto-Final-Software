USE lms_db      
GO




--  SP para insertar Entrega Modificado - (Valida que no se envie dos veces)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_InsertarEntregaTarea
    @evaluacion_id INT,
    @estudiante_id INT,
    @archivo_url NVARCHAR(255) = NULL,
    @contenido_texto NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF (LTRIM(RTRIM(@archivo_url)) = '' OR @archivo_url IS NULL) AND (LTRIM(RTRIM(@contenido_texto)) = '' OR @contenido_texto IS NULL)
        BEGIN ;THROW 50001, 'Error: Debe enviar un archivo o proporcionar contenido de texto.', 1; RETURN; END

    IF EXISTS (SELECT 1 FROM Evaluaciones.entregas_tareas WHERE evaluacion_id = @evaluacion_id AND estudiante_id = @estudiante_id)
        BEGIN ;THROW 50001, 'Error: Ya has realizado una entrega para esta evaluación.', 1; RETURN; END

    BEGIN TRY
        INSERT INTO Evaluaciones.entregas_tareas (evaluacion_id, estudiante_id, archivo_url, contenido_texto, fecha_entrega, estado)
        OUTPUT INSERTED.id
        VALUES (@evaluacion_id, @estudiante_id, @archivo_url, @contenido_texto, GETDATE(), 'Enviado');
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al registrar entrega. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO




-- SP Actualizar Entrega - Modificado - (Validando calificación positiva)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ActualizarEntregaTarea
    @id INT,
    @calificacion DECIMAL(5,2),
    @retroalimentacion NVARCHAR(MAX),
    @estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @calificacion < 0
        BEGIN ;THROW 50001, 'Error: La calificación no puede ser negativa.', 1; RETURN; END

    IF @estado NOT IN ('Enviado', 'Calificado', 'Revisión')
        BEGIN ;THROW 50001, 'Error: Estado inválido.', 1; RETURN; END

    BEGIN TRY
        UPDATE Evaluaciones.entregas_tareas
        SET calificacion = @calificacion, retroalimentacion = @retroalimentacion, estado = @estado
        WHERE id = @id;

        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La entrega que intenta calificar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar entrega. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO



-- SP para Eliminar Entrega

CREATE OR ALTER PROCEDURE Evaluaciones.SP_EliminarEntregaTarea
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Evaluaciones.entregas_tareas WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La entrega que intenta eliminar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar la entrega. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO



-- SP para listar Entregas por Evaluación - Modificado - (Se agrega el JOIN a estudiantes para React)

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ListarEntregasPorEvaluacion
    @evaluacion_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            e.id, e.evaluacion_id, e.estudiante_id, 
            est.nombre + ' ' + est.apellido AS estudiante_nombre,
            e.archivo_url, e.contenido_texto, e.fecha_entrega, e.calificacion, e.retroalimentacion, e.estado
        FROM Evaluaciones.entregas_tareas e
        INNER JOIN Usuarios.estudiantes est ON e.estudiante_id = est.id
        WHERE e.evaluacion_id = @evaluacion_id
        ORDER BY e.fecha_entrega DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar entregas. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO