USE lms_db
GO

-- SP PARA INSERTAR DOCENTE

CREATE OR ALTER PROCEDURE Usuarios.SP_InsertarDocente
    @usuario_id INT,
    @codigo_docente NVARCHAR(50),
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @especialidad NVARCHAR(150),
    @telefono NVARCHAR(20),
    @biografia NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@nombre)) = '' OR @nombre IS NULL OR LTRIM(RTRIM(@apellido)) = '' OR @apellido IS NULL
    BEGIN
        ;THROW 50001, 'Error: El nombre y apellido son obligatorios.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Usuarios.docentes (usuario_id, codigo_docente, nombre, apellido, especialidad, telefono, biografia, creado_en)
        OUTPUT INSERTED.id
        VALUES (@usuario_id, @codigo_docente, @nombre, @apellido, @especialidad, @telefono, @biografia, GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar docente. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP PARA ACTUALIZAR DOCENTES
CREATE OR ALTER PROCEDURE Usuarios.SP_ActualizarDocente
    @id INT,
    @codigo_docente NVARCHAR(50),
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @especialidad NVARCHAR(150),
    @telefono NVARCHAR(20),
    @biografia NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@nombre)) = '' OR LTRIM(RTRIM(@apellido)) = ''
    BEGIN
        ;THROW 50001, 'Error: El nombre y apellido no pueden estar vacíos.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Usuarios.docentes
        SET codigo_docente = @codigo_docente,
            nombre = @nombre,
            apellido = @apellido,
            especialidad = @especialidad,
            telefono = @telefono,
            biografia = @biografia
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El docente que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar docente. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

--  SP PARA Eliminar Docente (Borrado físico)

CREATE OR ALTER PROCEDURE Usuarios.SP_EliminarDocente
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Usuarios.docentes
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El docente que intenta eliminar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar docente. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP PARA Listar Docentes

CREATE OR ALTER PROCEDURE Usuarios.SP_ListarDocentes
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, usuario_id, codigo_docente, nombre, apellido, especialidad, telefono, biografia, creado_en
        FROM Usuarios.docentes
        ORDER BY apellido ASC, nombre ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar docentes.', 1;
    END CATCH
END;
GO