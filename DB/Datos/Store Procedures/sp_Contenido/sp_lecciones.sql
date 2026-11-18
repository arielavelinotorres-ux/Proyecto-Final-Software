USE lms_db
GO



-- 1. Insertar Lección
CREATE OR ALTER PROCEDURE Contenido.sp_InsertarLeccion
    @seccion_id INT,
    @titulo NVARCHAR(150),
    @contenido_texto NVARCHAR(MAX) = NULL,
    @url_video NVARCHAR(255) = NULL,
    @duracion_minutos INT = 0,
    @orden INT,
    @es_gratuita BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el título no vaya vacío
    IF LTRIM(RTRIM(ISNULL(@titulo, ''))) = ''
    BEGIN
        RAISERROR('El título de la lección es obligatorio y no puede estar vacío.', 16, 1);
        RETURN;
    END

    -- Validar que la duración y el orden no sean negativos
    IF @duracion_minutos < 0 OR @orden < 0
    BEGIN
        RAISERROR('La duración en minutos y el orden de la lección no pueden ser negativos.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Contenido.lecciones (seccion_id, titulo, contenido_texto, url_video, duracion_minutos, orden, es_gratuita)
        OUTPUT INSERTED.id AS leccion_id
        VALUES (@seccion_id, @titulo, @contenido_texto, @url_video, @duracion_minutos, @orden, @es_gratuita);
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 2. Consultar Lecciones (con filtro opcional por @id o @seccion_id y ordenadas por orden ASC)
CREATE OR ALTER PROCEDURE Contenido.sp_ConsultarLecciones
    @id INT = NULL,
    @seccion_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id, seccion_id, titulo, contenido_texto, url_video, duracion_minutos, orden, es_gratuita, creado_en 
    FROM Contenido.lecciones
    WHERE (@id IS NULL OR id = @id)
      AND (@seccion_id IS NULL OR seccion_id = @seccion_id)
    ORDER BY orden ASC;
END;
GO

--------------------------------------------------------------------

-- 3. Actualizar Lección
CREATE OR ALTER PROCEDURE Contenido.sp_ActualizarLeccion
    @id INT,
    @seccion_id INT,
    @titulo NVARCHAR(150),
    @contenido_texto NVARCHAR(MAX) = NULL,
    @url_video NVARCHAR(255) = NULL,
    @duracion_minutos INT,
    @orden INT,
    @es_gratuita BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el título no vaya vacío
    IF LTRIM(RTRIM(ISNULL(@titulo, ''))) = ''
    BEGIN
        RAISERROR('El título de la lección es obligatorio y no puede estar vacío.', 16, 1);
        RETURN;
    END

    -- Validar que la duración y el orden no sean negativos
    IF @duracion_minutos < 0 OR @orden < 0
    BEGIN
        RAISERROR('La duración en minutos y el orden de la lección no pueden ser negativos.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        UPDATE Contenido.lecciones
        SET seccion_id = @seccion_id,
            titulo = @titulo,
            contenido_texto = @contenido_texto,
            url_video = @url_video,
            duracion_minutos = @duracion_minutos,
            orden = @orden,
            es_gratuita = @es_gratuita
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La lección especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 4. Eliminar Lección
CREATE OR ALTER PROCEDURE Contenido.sp_EliminarLeccion
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Contenido.lecciones 
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La lección especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('No se puede eliminar la lección porque está siendo utilizada en otros registros.', 16, 1);
        END
        ELSE
        BEGIN
            ;THROW;
        END
    END CATCH
END;
GO