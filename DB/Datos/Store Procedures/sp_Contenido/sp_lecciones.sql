USE lms_db
GO

-- 1. 
CREATE PROCEDURE Contenido.sp_InsertarLeccion
    @seccion_id INT,
    @titulo NVARCHAR(150),
    @contenido_texto NVARCHAR(MAX) = NULL,
    @url_video NVARCHAR(255) = NULL,
    @duracion_minutos INT = 0,
    @orden INT,
    @es_gratuita BIT = 0
AS
BEGIN
    INSERT INTO Contenido.lecciones (seccion_id, titulo, contenido_texto, url_video, duracion_minutos, orden, es_gratuita)
    VALUES (@seccion_id, @titulo, @contenido_texto, @url_video, @duracion_minutos, @orden, @es_gratuita);
END;
GO

-- 2. 
CREATE PROCEDURE Contenido.sp_ConsultarLecciones
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, seccion_id, titulo, contenido_texto, url_video, duracion_minutos, orden, es_gratuita, creado_en 
        FROM Contenido.lecciones;
    END
    ELSE
    BEGIN
        SELECT id, seccion_id, titulo, contenido_texto, url_video, duracion_minutos, orden, es_gratuita, creado_en 
        FROM Contenido.lecciones 
        WHERE id = @id;
    END
END;
GO

-- 3. 
CREATE PROCEDURE Contenido.sp_ActualizarLeccion
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
    UPDATE Contenido.lecciones
    SET seccion_id = @seccion_id,
        titulo = @titulo,
        contenido_texto = @contenido_texto,
        url_video = @url_video,
        duracion_minutos = @duracion_minutos,
        orden = @orden,
        es_gratuita = @es_gratuita
    WHERE id = @id;
END;
GO

-- 4.
CREATE PROCEDURE Contenido.sp_EliminarLeccion
    @id INT
AS
BEGIN
    DELETE FROM Contenido.lecciones 
    WHERE id = @id;
END;
GO