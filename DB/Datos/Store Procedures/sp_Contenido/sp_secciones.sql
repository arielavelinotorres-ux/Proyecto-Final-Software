USE lms_db
GO


CREATE PROCEDURE Contenido.sp_InsertarSeccion
    @curso_id INT,
    @titulo NVARCHAR(150),
    @orden INT
AS
BEGIN
    INSERT INTO Contenido.secciones (curso_id, titulo, orden)
    VALUES (@curso_id, @titulo, @orden);
END;
GO

CREATE PROCEDURE Contenido.sp_ConsultarSecciones
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, curso_id, titulo, orden, creado_en 
        FROM Contenido.secciones;
    END
    ELSE
    BEGIN
        SELECT id, curso_id, titulo, orden, creado_en 
        FROM Contenido.secciones 
        WHERE id = @id;
    END
END;
GO

CREATE PROCEDURE Contenido.sp_ActualizarSeccion
    @id INT,
    @curso_id INT,
    @titulo NVARCHAR(150),
    @orden INT
AS
BEGIN
    UPDATE Contenido.secciones
    SET curso_id = @curso_id,
        titulo = @titulo,
        orden = @orden
    WHERE id = @id;
END;
GO


CREATE PROCEDURE Contenido.sp_EliminarSeccion
    @id INT
AS
BEGIN
    DELETE FROM Contenido.secciones 
    WHERE id = @id;
END;
GO