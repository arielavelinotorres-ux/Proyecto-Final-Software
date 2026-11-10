USE lms_db
GO


CREATE PROCEDURE Contenido.sp_InsertarRecurso
    @leccion_id INT,
    @nombre NVARCHAR(150),
    @tipo_archivo NVARCHAR(50) = NULL,
    @url_recurso NVARCHAR(255)
AS
BEGIN
    INSERT INTO Contenido.recursos (leccion_id, nombre, tipo_archivo, url_recurso)
    VALUES (@leccion_id, @nombre, @tipo_archivo, @url_recurso);
END;
GO


CREATE PROCEDURE Contenido.sp_ConsultarRecursos
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, leccion_id, nombre, tipo_archivo, url_recurso, creado_en 
        FROM Contenido.recursos;
    END
    ELSE
    BEGIN
        SELECT id, leccion_id, nombre, tipo_archivo, url_recurso, creado_en 
        FROM Contenido.recursos 
        WHERE id = @id;
    END
END;
GO


CREATE PROCEDURE Contenido.sp_ActualizarRecurso
    @id INT,
    @leccion_id INT,
    @nombre NVARCHAR(150),
    @tipo_archivo NVARCHAR(50) = NULL,
    @url_recurso NVARCHAR(255)
AS
BEGIN
    UPDATE Contenido.recursos
    SET leccion_id = @leccion_id,
        nombre = @nombre,
        tipo_archivo = @tipo_archivo,
        url_recurso = @url_recurso
    WHERE id = @id;
END;
GO


CREATE PROCEDURE Contenido.sp_EliminarRecurso
    @id INT
AS
BEGIN
    DELETE FROM Contenido.recursos 
    WHERE id = @id;
END;
GO