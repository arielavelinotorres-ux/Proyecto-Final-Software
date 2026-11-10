USE lms_db
GO



-- SP para insertar certificado

CREATE OR ALTER PROCEDURE Monitoreo.SP_InsertarCertificado
    @estudiante_id INT,
    @curso_id INT,
    @codigo_validacion NVARCHAR(50),
    @url_pdf NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@codigo_validacion)) = '' OR @codigo_validacion IS NULL
    BEGIN
        ;THROW 50001, 'Error: El código de validación es obligatorio.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Monitoreo.certificados (estudiante_id, curso_id, codigo_validacion, fecha_emision, url_pdf)
        OUTPUT INSERTED.id
        VALUES (@estudiante_id, @curso_id, @codigo_validacion, GETDATE(), @url_pdf);
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar certificado. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO






-- SP actualizar certificado (Principalmente para actualizar PDF)

CREATE OR ALTER PROCEDURE Monitoreo.SP_ActualizarCertificado
    @id INT,
    @codigo_validacion NVARCHAR(50),
    @url_pdf NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@codigo_validacion)) = ''
    BEGIN
        ;THROW 50001, 'Error: El código de validación no puede estar vacío.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Monitoreo.certificados
        SET codigo_validacion = @codigo_validacion,
            url_pdf = @url_pdf
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El certificado que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al actualizar certificado.', 1;
    END CATCH
END;
GO





-- SP para eliminar certificado (Borrado físico)

CREATE OR ALTER PROCEDURE Monitoreo.SP_EliminarCertificado
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Monitoreo.certificados WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El certificado no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al eliminar certificado.', 1;
    END CATCH
END;
GO





-- SP para listar certificados por estudiante

CREATE OR ALTER PROCEDURE Monitoreo.SP_ListarCertificadosPorEstudiante
    @estudiante_id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, estudiante_id, curso_id, codigo_validacion, fecha_emision, url_pdf
        FROM Monitoreo.certificados
        WHERE estudiante_id = @estudiante_id
        ORDER BY fecha_emision DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar certificados.', 1;
    END CATCH
END;
GO