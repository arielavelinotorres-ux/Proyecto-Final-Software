USE lms_db
GO


-- 1. Insertar Sección
CREATE OR ALTER PROCEDURE Contenido.sp_InsertarSeccion
    @curso_id INT,
    @titulo NVARCHAR(150),
    @orden INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el título no vaya vacío
    IF LTRIM(RTRIM(ISNULL(@titulo, ''))) = ''
    BEGIN
        RAISERROR('El título de la sección es obligatorio y no puede estar vacío.', 16, 1);
        RETURN;
    END

    -- Validar que el orden no sea negativo
    IF @orden < 0
    BEGIN
        RAISERROR('El orden de la sección no puede ser negativo.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Contenido.secciones (curso_id, titulo, orden)
        OUTPUT INSERTED.id AS seccion_id
        VALUES (@curso_id, @titulo, @orden);
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 2. Consultar Secciones (con filtro opcional por @id o @curso_id y ordenadas por orden ASC)
CREATE OR ALTER PROCEDURE Contenido.sp_ConsultarSecciones
    @id INT = NULL,
    @curso_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, curso_id, titulo, orden, creado_en 
    FROM Contenido.secciones
    WHERE (@id IS NULL OR id = @id)
      AND (@curso_id IS NULL OR curso_id = @curso_id)
    ORDER BY orden ASC;
END;
GO

--------------------------------------------------------------------

-- 3. Actualizar Sección
CREATE OR ALTER PROCEDURE Contenido.sp_ActualizarSeccion
    @id INT,
    @curso_id INT,
    @titulo NVARCHAR(150),
    @orden INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el título no vaya vacío
    IF LTRIM(RTRIM(ISNULL(@titulo, ''))) = ''
    BEGIN
        RAISERROR('El título de la sección es obligatorio y no puede estar vacío.', 16, 1);
        RETURN;
    END

    -- Validar que el orden no sea negativo
    IF @orden < 0
    BEGIN
        RAISERROR('El orden de la sección no puede ser negativo.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        UPDATE Contenido.secciones
        SET curso_id = @curso_id,
            titulo = @titulo,
            orden = @orden
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La sección especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 4. Eliminar Sección
CREATE OR ALTER PROCEDURE Contenido.sp_EliminarSeccion
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Contenido.secciones 
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La sección especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('No se puede eliminar la sección porque contiene lecciones u otros registros asociados.', 16, 1);
        END
        ELSE
        BEGIN
            ;THROW;
        END
    END CATCH
END;
GO