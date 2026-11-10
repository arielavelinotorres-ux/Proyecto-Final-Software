USE lms_db
GO




-- SP para insertar progreso (Inicia una lección)

CREATE OR ALTER PROCEDURE Monitoreo.SP_InsertarProgresoLeccion
    @estudiante_id INT,
    @leccion_id INT,
    @completado BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @fecha_completado DATETIME2 = NULL;

    IF @completado = 1
    BEGIN
        SET @fecha_completado = GETDATE();
    END

    BEGIN TRY
        INSERT INTO Monitoreo.progreso_lecciones (estudiante_id, leccion_id, completado, fecha_completado)
        OUTPUT INSERTED.id
        VALUES (@estudiante_id, @leccion_id, @completado, @fecha_completado);
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al registrar progreso. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO






-- SP para actualizar progreso (Marcar como completado/incompleto)

CREATE OR ALTER PROCEDURE Monitoreo.SP_ActualizarProgresoLeccion
    @id INT,
    @completado BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @fecha_completado DATETIME2 = NULL;

    IF @completado = 1
    BEGIN
        SET @fecha_completado = GETDATE();
    END

    BEGIN TRY
        UPDATE Monitoreo.progreso_lecciones
        SET completado = @completado,
            fecha_completado = @fecha_completado
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El registro de progreso no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al actualizar progreso.', 1;
    END CATCH
END;
GO




-- SP para eliminar progreso

CREATE OR ALTER PROCEDURE Monitoreo.SP_EliminarProgresoLeccion
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Monitoreo.progreso_lecciones WHERE id = @id;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al eliminar progreso.', 1;
    END CATCH
END;
GO





-- SP para listar progreso por estudiante

CREATE OR ALTER PROCEDURE Monitoreo.SP_ListarProgresoPorEstudiante
    @estudiante_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, estudiante_id, leccion_id, completado, fecha_completado
        FROM Monitoreo.progreso_lecciones
        WHERE estudiante_id = @estudiante_id
        ORDER BY id ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar progreso.', 1;
    END CATCH
END;
GO