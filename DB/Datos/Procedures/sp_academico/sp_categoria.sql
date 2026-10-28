USE lms_db;
GO

CREATE PROCEDURE Academico.sp_InsertarCategoria
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT = 1
AS
BEGIN
    INSERT INTO Academico.categorias (nombre, descripcion, activo)
    VALUES (@nombre, @descripcion, @activo);
END;
GO
--------------------------------------------------------------------

CREATE PROCEDURE Academico.sp_ConsultarCategorias
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, nombre, descripcion, activo, creado_en
        FROM Academico.categorias;
    END
    ELSE
    BEGIN
        SELECT id, nombre, descripcion, activo, creado_en
        FROM Academico.categorias
        WHERE id = @id;
    END
END;
GO

-------------------------------------------------------------------
CREATE PROCEDURE Academico.sp_ActualizarCategoria
    @id INT,
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT
AS
BEGIN
    UPDATE Academico.categorias
    SET nombre = @nombre,
        descripcion = @descripcion,
        activo = @activo
    WHERE id = @id;
END;
GO
----------------------------------------------------------------

CREATE PROCEDURE Academico.sp_EliminarCategoria
    @id INT
AS
BEGIN
    DELETE FROM Academico.categorias
    WHERE id = @id;
END;
GO