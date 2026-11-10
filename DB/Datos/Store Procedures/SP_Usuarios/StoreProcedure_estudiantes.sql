USE lms_db
GO

-- SP PARA Insertar Estudiantes

CREATE OR ALTER PROCEDURE Usuarios.SP_InsertarEstudiante
    @usuario_id INT,
    @codigo_estudiante NVARCHAR(50),
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @fecha_nacimiento DATE,
    @telefono NVARCHAR(20),
    @contacto_emergencia NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@nombre)) = '' OR @nombre IS NULL OR LTRIM(RTRIM(@apellido)) = '' OR @apellido IS NULL
    BEGIN
        ;THROW 50001, 'Error: El nombre y apellido son obligatorios.', 1;
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Usuarios.estudiantes (usuario_id, codigo_estudiante, nombre, apellido, fecha_nacimiento, telefono, contacto_emergencia, creado_en)
        OUTPUT INSERTED.id
        VALUES (@usuario_id, @codigo_estudiante, @nombre, @apellido, @fecha_nacimiento, @telefono, @contacto_emergencia, GETDATE());
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al insertar estudiante. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP PARA Actualizar Estudiantes

CREATE OR ALTER PROCEDURE Usuarios.SP_ActualizarEstudiante
    @id INT,
    @codigo_estudiante NVARCHAR(50),
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @fecha_nacimiento DATE,
    @telefono NVARCHAR(20),
    @contacto_emergencia NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    IF LTRIM(RTRIM(@nombre)) = '' OR LTRIM(RTRIM(@apellido)) = ''
    BEGIN
        ;THROW 50001, 'Error: El nombre y apellido no pueden estar vacíos.', 1;
        RETURN;
    END

    BEGIN TRY
        UPDATE Usuarios.estudiantes
        SET codigo_estudiante = @codigo_estudiante,
            nombre = @nombre,
            apellido = @apellido,
            fecha_nacimiento = @fecha_nacimiento,
            telefono = @telefono,
            contacto_emergencia = @contacto_emergencia
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El estudiante que intenta actualizar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al actualizar estudiante. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP para Eliminar Estudiante (Borrado físico)

CREATE OR ALTER PROCEDURE Usuarios.SP_EliminarEstudiante
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Usuarios.estudiantes
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 50003, 'Error: El estudiante que intenta eliminar no existe.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al eliminar estudiante. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

-- SP PARA Listar Estudiantes
CREATE OR ALTER PROCEDURE Usuarios.SP_ListarEstudiantes
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, usuario_id, codigo_estudiante, nombre, apellido, fecha_nacimiento, telefono, contacto_emergencia, creado_en
        FROM Usuarios.estudiantes
        ORDER BY apellido ASC, nombre ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar estudiantes.', 1;
    END CATCH
END;
GO