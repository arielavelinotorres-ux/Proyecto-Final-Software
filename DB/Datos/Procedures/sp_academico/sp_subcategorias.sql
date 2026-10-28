USE lms_db;
GO

CREATE PROCEDURE Academico.sp_InsertarSubcategoria
    @categoria_id INT,
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT = 1
AS
BEGIN
    INSERT INTO Academico.subcategorias (categoria_id, nombre, descripcion, activo)
    VALUES (@categoria_id, @nombre, @descripcion, @activo);
END;
GO

EXEC Academico.sp_InsertarSubcategoria 
    @categoria_id = 1, 
    @nombre = 'Álgebra', 
    @descripcion = 'Subcategoría de matemáticas enfocada en álgebra básica.', 
    @activo = 1;


    CREATE PROCEDURE Academico.sp_ConsultarSubcategorias
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, categoria_id, nombre, descripcion, activo, creado_en 
        FROM Academico.subcategorias;
    END
    ELSE
    BEGIN
        SELECT id, categoria_id, nombre, descripcion, activo, creado_en 
        FROM Academico.subcategorias 
        WHERE id = @id;
    END
END;
GO


CREATE PROCEDURE Academico.sp_ActualizarSubcategoria
    @id INT,
    @categoria_id INT,
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT
AS
BEGIN
    UPDATE Academico.subcategorias
    SET categoria_id = @categoria_id,
        nombre = @nombre,
        descripcion = @descripcion,
        activo = @activo
    WHERE id = @id;
END;
GO

EXEC Academico.sp_ActualizarSubcategoria 
    @id = 1, 
    @categoria_id = 1, 
    @nombre = 'Álgebra Avanzada', 
    @descripcion = 'Descripción actualizada.', 
    @activo = 1;



    CREATE PROCEDURE Academico.sp_EliminarSubcategoria
    @id INT
AS
BEGIN
    DELETE FROM Academico.subcategorias 
    WHERE id = @id;
END;
GO