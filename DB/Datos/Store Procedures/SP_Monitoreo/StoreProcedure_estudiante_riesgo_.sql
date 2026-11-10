USE lms_db
GO




-- SP para insertar registro de riesgo

CREATE OR ALTER PROCEDURE Monitoreo.SP_InsertarEstudianteRiesgo
    @estudiante_id INT,
    @curso_id INT,
    @nivel_riesgo NVARCHAR(20),
    @motivo NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF @nivel_riesgo NOT IN ('Bajo', 'Medio', 'Alto', 'Critico')
    BEGIN
        ;THROW 50001, 'Error: Nivel de riesgo inválido.', 1;
        RETURN;
    END

    IF LTRIM(RTRIM(@motivo)) = '' OR @motivo IS NULL
    BEGIN
        ;THROW 50001, 'Error: El motivo es obligatorio.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Monitoreo.estudiantes_riesgo (estudiante_id, curso_id, nivel_riesgo, motivo, detectado_en, atendido)
        OUTPUT INSERTED.id
        VALUES (@estudiante_id, @curso_id, @nivel_riesgo, @motivo, GETDATE(), 0);
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al registrar estudiante en riesgo. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO





-- SP para actualizar riesgo (Cambiar nivel, motivo o marcar como atendido)

CREATE OR ALTER PROCEDURE Monitoreo.SP_ActualizarEstudianteRiesgo
    @id INT,
    @nivel_riesgo NVARCHAR(20),
    @motivo NVARCHAR(255),
    @atendido BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF @nivel_riesgo NOT IN ('Bajo', 'Medio', 'Alto', 'Critico')
    BEGIN
        ;THROW 50001, 'Error: Nivel de riesgo inválido.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Monitoreo.estudiantes_riesgo
        SET nivel_riesgo = @nivel_riesgo,
            motivo = @motivo,
            atendido = @atendido
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El registro de riesgo no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al actualizar registro de riesgo.', 1;
    END CATCH
END;
GO




-- SP para eliminar registro de riesgo

CREATE OR ALTER PROCEDURE Monitoreo.SP_EliminarEstudianteRiesgo
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Monitoreo.estudiantes_riesgo WHERE id = @id;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al eliminar registro de riesgo.', 1;
    END CATCH
END;
GO





-- SP para listar estudiantes en riesgo (Filtro general o por curso)

CREATE OR ALTER PROCEDURE Monitoreo.SP_ListarEstudiantesRiesgo
    @curso_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, estudiante_id, curso_id, nivel_riesgo, motivo, detectado_en, atendido
        FROM Monitoreo.estudiantes_riesgo
        WHERE (@curso_id IS NULL OR curso_id = @curso_id)
        ORDER BY atendido ASC, detectado_en DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar registros de riesgo.', 1;
    END CATCH
END;
GO