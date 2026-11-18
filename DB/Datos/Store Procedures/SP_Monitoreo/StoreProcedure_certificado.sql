USE lms_db
GO



-- SP para insertar certificado (Se agrega la validacion de duplicados)

CREATE OR ALTER PROCEDURE Monitoreo.SP_InsertarCertificado
    @estudiante_id INT, @curso_id INT, @codigo_validacion NVARCHAR(50), @url_pdf NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@codigo_validacion)) = '' OR @codigo_validacion IS NULL
        BEGIN ;THROW 50001, 'Error: El código de validación es obligatorio.', 1; RETURN; END

    IF EXISTS (SELECT 1 FROM Monitoreo.certificados WHERE estudiante_id = @estudiante_id AND curso_id = @curso_id)
        BEGIN ;THROW 50001, 'Error: El estudiante ya tiene un certificado para este curso.', 1; RETURN; END

    IF EXISTS (SELECT 1 FROM Monitoreo.certificados WHERE codigo_validacion = @codigo_validacion)
        BEGIN ;THROW 50001, 'Error: El código de validación ya está en uso.', 1; RETURN; END

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



-- SP actualizar certificado - MODIFICADO

CREATE OR ALTER PROCEDURE Monitoreo.SP_ActualizarCertificado
    @id INT, @codigo_validacion NVARCHAR(50), @url_pdf NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@codigo_validacion)) = '' OR @codigo_validacion IS NULL
        BEGIN ;THROW 50001, 'Error: El código de validación no puede estar vacío.', 1; RETURN; END

    IF EXISTS (SELECT 1 FROM Monitoreo.certificados WHERE codigo_validacion = @codigo_validacion AND id <> @id)
        BEGIN ;THROW 50001, 'Error: El código de validación ya pertenece a otro certificado.', 1; RETURN; END

    BEGIN TRY
        UPDATE Monitoreo.certificados SET codigo_validacion = @codigo_validacion, url_pdf = @url_pdf WHERE id = @id;
        IF @@ROWCOUNT = 0
            BEGIN ;THROW 50003, 'Error: El certificado que intenta actualizar no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar certificado. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO



-- SP NUEVO Valida Certificados Por Código - (Para la página de verificación pública)

CREATE OR ALTER PROCEDURE Monitoreo.SP_ValidarCertificadoPorCodigo
    @codigo_validacion NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            c.id, c.codigo_validacion, c.fecha_emision, c.url_pdf,
            e.nombre + ' ' + e.apellido AS estudiante_nombre,
            cur.titulo AS curso_titulo
        FROM Monitoreo.certificados c
        INNER JOIN Usuarios.estudiantes e ON c.estudiante_id = e.id
        INNER JOIN Academico.cursos cur ON c.curso_id = cur.id
        WHERE c.codigo_validacion = @codigo_validacion;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al validar certificado. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
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
            BEGIN ;THROW 50003, 'Error: El certificado no existe.', 1; END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar certificado. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO




-- SP para listar certificados por estudiante - MODIFICADO (Se agrega el JOIN al nombre del curso)

CREATE OR ALTER PROCEDURE Monitoreo.SP_ListarCertificadosPorEstudiante
    @estudiante_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            c.id, c.estudiante_id, c.curso_id, cur.titulo AS curso_titulo,
            c.codigo_validacion, c.fecha_emision, c.url_pdf
        FROM Monitoreo.certificados c
        INNER JOIN Academico.cursos cur ON c.curso_id = cur.id
        WHERE c.estudiante_id = @estudiante_id
        ORDER BY c.fecha_emision DESC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar certificados. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO