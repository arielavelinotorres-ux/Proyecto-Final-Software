USE lms_db
GO

-- ====================================================================
-- CRUD: Configuracion.parametros_sistema
-- ====================================================================

-- 1. Insertar Parámetro de Sistema
CREATE OR ALTER PROCEDURE Configuracion.sp_InsertarParametroSistema
    @clave NVARCHAR(100),
    @valor NVARCHAR(MAX),
    @descripcion NVARCHAR(255) = NULL,
    @tipo_dato NVARCHAR(20) = 'Texto',
    @usuario_id INT = NULL -- Opcional para la auditoría inicial si aplica
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la clave no venga vacía o nula
    IF LTRIM(RTRIM(ISNULL(@clave, ''))) = ''
    BEGIN
        RAISERROR('La clave del parámetro es obligatoria y no puede estar vacía.', 16, 1);
        RETURN;
    END

    -- Validar si la clave ya existe
    IF EXISTS (SELECT 1 FROM Configuracion.parametros_sistema WHERE clave = @clave)
    BEGIN
        RAISERROR('La clave del parámetro ya se encuentra registrada.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        DECLARE @NuevoId INT;

        INSERT INTO Configuracion.parametros_sistema (clave, valor, descripcion, tipo_dato)
        OUTPUT INSERTED.id INTO @NuevoId(id) -- O usando INSERTED.id directamente con SCOPE_IDENTITY() o variable tabla
        VALUES (@clave, @valor, @descripcion, @tipo_dato);
        
        -- Como INSERTED.id se puede capturar con SELECT o variable de tabla:
        SET @NuevoId = SCOPE_IDENTITY();

        -- Opcional: Registrar auditoría de la creación si se provee usuario_id
        IF @usuario_id IS NOT NULL
        BEGIN
            INSERT INTO Configuracion.auditoria_configuraciones (parametro_id, usuario_id, valor_anterior, valor_nuevo)
            VALUES (@NuevoId, @usuario_id, NULL, @valor);
        END

        -- Retornar el ID creado
        SELECT @NuevoId AS parametro_id;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            RAISERROR('La clave del parámetro ya se encuentra registrada.', 16, 1);
            RETURN;
        END

        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 2. Consultar Parámetros de Sistema (permite filtrar por @id o por @clave)
CREATE OR ALTER PROCEDURE Configuracion.sp_ConsultarParametrosSistema
    @id INT = NULL,
    @clave NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, clave, valor, descripcion, tipo_dato, actualizado_en 
    FROM Configuracion.parametros_sistema
    WHERE (@id IS NULL OR id = @id)
      AND (@clave IS NULL OR clave = @clave);
END;
GO

--------------------------------------------------------------------

-- 3. Actualizar Parámetro de Sistema (registra automáticamente en auditoría)
CREATE OR ALTER PROCEDURE Configuracion.sp_ActualizarParametroSistema
    @id INT,
    @clave NVARCHAR(100),
    @valor NVARCHAR(MAX),
    @descripcion NVARCHAR(255) = NULL,
    @tipo_dato NVARCHAR(20),
    @usuario_id INT = NULL -- Necesario para registrar quién realizó el cambio en la auditoría
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la clave no venga vacía o nula
    IF LTRIM(RTRIM(ISNULL(@clave, ''))) = ''
    BEGIN
        RAISERROR('La clave del parámetro es obligatoria y no puede estar vacía.', 16, 1);
        RETURN;
    END

    -- Validar si la clave ya pertenece a otro registro
    IF EXISTS (SELECT 1 FROM Configuracion.parametros_sistema WHERE clave = @clave AND id <> @id)
    BEGIN
        RAISERROR('La clave del parámetro ya se encuentra registrada en otro registro.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        -- Obtener el valor anterior para la auditoría
        DECLARE @valor_anterior NVARCHAR(MAX);
        SELECT @valor_anterior = valor FROM Configuracion.parametros_sistema WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('El parámetro del sistema especificado no existe.', 16, 1);
            RETURN;
        END

        -- Actualizar el parámetro
        UPDATE Configuracion.parametros_sistema
        SET clave = @clave,
            valor = @valor,
            descripcion = @descripcion,
            tipo_dato = @tipo_dato,
            actualizado_en = GETDATE() -- Actualiza la marca de tiempo automáticamente
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('El parámetro del sistema especificado no existe.', 16, 1);
            RETURN;
        END

        -- Guardar automáticamente el registro en auditoria_configuraciones si cambió el valor y se proporcionó el usuario
        IF ISNULL(@valor_anterior, '') <> ISNULL(@valor, '') AND @usuario_id IS NOT NULL
        BEGIN
            INSERT INTO Configuracion.auditoria_configuraciones (parametro_id, usuario_id, valor_anterior, valor_nuevo)
            VALUES (@id, @usuario_id, @valor_anterior, @valor);
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            RAISERROR('La clave del parámetro ya se encuentra registrada.', 16, 1);
            RETURN;
        END

        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 4. Eliminar Parámetro de Sistema
CREATE OR ALTER PROCEDURE Configuracion.sp_EliminarParametroSistema
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Configuracion.parametros_sistema 
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('El parámetro del sistema especificado no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('No se puede eliminar el parámetro del sistema porque está siendo utilizado en otros registros.', 16, 1);
        END
        ELSE
        BEGIN
            ;THROW;
        END
    END CATCH
END;
GO