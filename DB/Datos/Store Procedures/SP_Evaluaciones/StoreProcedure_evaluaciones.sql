Use lms_db
GO 

CREATE OR ALTER PROCEDURE Evaluaciones.SP_InsertarEvaluacion
    @curso_id INT,
    @titulo NVARCHAR(150),
    @descripcion NVARCHAR(MAX) = NULL,
    @tipo NVARCHAR(30),
    @puntaje_maximo DECIMAL(5,2) = 100.00,
    @fecha_inicio DATETIME2 = NULL,
    @fecha_limite DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@titulo)) = '' OR @titulo IS NULL
        BEGIN ;THROW 50001, 'Error: El título de la evaluación no puede estar vacío.', 1; RETURN; END

    IF @tipo NOT IN ('Tarea', 'Examen', 'Cuestionario', 'Proyecto')
        BEGIN ;THROW 50001, 'Error: Tipo de evaluación no válido.', 1; RETURN; END

    IF @puntaje_maximo <= 0
        BEGIN ;THROW 50001, 'Error: El puntaje máximo debe ser mayor a cero.', 1; RETURN; END

    IF @fecha_inicio IS NOT NULL AND @fecha_limite IS NOT NULL AND @fecha_limite < @fecha_inicio
        BEGIN ;THROW 50001, 'Error: La fecha límite no puede ser anterior a la fecha de inicio.', 1; RETURN; END

    BEGIN TRY
        INSERT INTO Evaluaciones.evaluaciones (curso_id, titulo, descripcion, tipo, puntaje_maximo, fecha_inicio, fecha_limite, estado, creado_en)
        OUTPUT INSERTED.id
        VALUES (@curso_id, @titulo, @descripcion, @tipo, @puntaje_maximo, ISNULL(@fecha_inicio, GETDATE()), @fecha_limite, 'Activo', GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar evaluación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ActualizarEvaluacion
    @id INT,
    @titulo NVARCHAR(150),
    @descripcion NVARCHAR(MAX) = NULL,
    @tipo NVARCHAR(30),
    @puntaje_maximo DECIMAL(5,2),
    @fecha_inicio DATETIME2 = NULL,
    @fecha_limite DATETIME2 = NULL,
    @estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@titulo)) = '' OR @titulo IS NULL
        BEGIN ;THROW 50001, 'Error: El título de la evaluación no puede estar vacío.', 1; RETURN; END

    IF @tipo NOT IN ('Tarea', 'Examen', 'Cuestionario', 'Proyecto')
        BEGIN ;THROW 50001, 'Error: Tipo de evaluación no válido.', 1; RETURN; END

    IF @puntaje_maximo <= 0
        BEGIN ;THROW 50001, 'Error: El puntaje máximo debe ser mayor a cero.', 1; RETURN; END

    IF @estado NOT IN ('Activo', 'Cerrado', 'Archivado')
        BEGIN ;THROW 50001, 'Error: Estado inválido. Solo se permite Activo, Cerrado o Archivado.', 1; RETURN; END

    IF @fecha_inicio IS NOT NULL AND @fecha_limite IS NOT NULL AND @fecha_limite < @fecha_inicio
        BEGIN ;THROW 50001, 'Error: La fecha límite no puede ser anterior a la fecha de inicio.', 1; RETURN; END

    BEGIN TRY
        UPDATE Evaluaciones.evaluaciones
        SET titulo = @titulo,
            descripcion = @descripcion,
            tipo = @tipo,
            puntaje_maximo = @puntaje_maximo,
            fecha_inicio = ISNULL(@fecha_inicio, fecha_inicio),
            fecha_limite = @fecha_limite,
            estado = @estado
        WHERE id = @id;

        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La evaluación que intenta actualizar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar evaluación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_DesactivarEvaluacion
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Evaluaciones.evaluaciones SET estado = 'Archivado' WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: La evaluación que intenta archivar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al archivar evaluación. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE Evaluaciones.SP_ListarEvaluacionesPorCurso
    @curso_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT id, curso_id, titulo, descripcion, tipo, puntaje_maximo, fecha_inicio, fecha_limite, estado, creado_en
        FROM Evaluaciones.evaluaciones
        WHERE curso_id = @curso_id AND estado != 'Archivado'
        ORDER BY creado_en DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar evaluaciones. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO