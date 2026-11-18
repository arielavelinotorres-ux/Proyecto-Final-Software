USE lms_db
GO


-- 1. Insertar Recurso
CREATE OR ALTER PROCEDURE Contenido.sp_InsertarRecurso
    @leccion_id INT,
    @nombre NVARCHAR(150),
    @tipo_archivo NVARCHAR(50) = NULL,
    @url_recurso NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el nombre y la URL del recurso no vengan vacíos o nulos
    IF LTRIM(RTRIM(ISNULL(@nombre, ''))) = '' OR LTRIM(RTRIM(ISNULL(@url_recurso, ''))) = ''
    BEGIN
        RAISERROR('El nombre y la URL del recurso son obligatorios y no pueden estar vacíos.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Contenido.recursos (leccion_id, nombre, tipo_archivo, url_recurso)
        OUTPUT INSERTED.id AS recurso_id
        VALUES (@leccion_id, @nombre, @tipo_archivo, @url_recurso);
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 2. Consultar Recursos (con filtros opcionales por @id o @leccion_id)
CREATE OR ALTER PROCEDURE Contenido.sp_ConsultarRecursos
    @id INT = NULL,
    @leccion_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, leccion_id, nombre, tipo_archivo, url_recurso, creado_en 
    FROM Contenido.recursos
    WHERE (@id IS NULL OR id = @id)
      AND (@leccion_id IS NULL OR leccion_id = @leccion_id);
END;
GO

--------------------------------------------------------------------

-- 3. Actualizar Recurso
CREATE OR ALTER PROCEDURE Contenido.sp_ActualizarRecurso
    @id INT,
    @leccion_id INT,
    @nombre NVARCHAR(150),
    @tipo_archivo NVARCHAR(50) = NULL,
    @url_recurso NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el nombre y la URL del recurso no vengan vacíos o nulos
    IF LTRIM(RTRIM(ISNULL(@nombre, ''))) = '' OR LTRIM(RTRIM(ISNULL(@url_recurso, ''))) = ''
    BEGIN
        RAISERROR('El nombre y la URL del recurso son obligatorios y no pueden estar vacíos.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        UPDATE Contenido.recursos
        SET leccion_id = @leccion_id,
            nombre = @nombre,
            tipo_archivo = @tipo_archivo,
            url_recurso = @url_recurso
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('El recurso especificado no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 4. Eliminar Recurso
CREATE OR ALTER PROCEDURE Contenido.sp_EliminarRecurso
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Contenido.recursos 
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('El recurso especificado no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('No se puede eliminar el recurso porque está siendo utilizado en otros registros.', 16, 1);
        END
        ELSE
        BEGIN
            ;THROW;
        END
    END CATCH
END;
GO