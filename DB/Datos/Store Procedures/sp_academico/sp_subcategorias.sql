USE lms_db;
GO


CREATE OR ALTER PROCEDURE Academico.sp_InsertarSubcategoria
    @categoria_id INT,
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Academico.subcategorias (categoria_id, nombre, descripcion, activo)
        OUTPUT INSERTED.id AS subcategoria_id
        VALUES (@categoria_id, @nombre, @descripcion, @activo);
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ConsultarSubcategorias
    @id INT = NULL,
    @categoria_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.id, 
        s.categoria_id, 
        c.nombre AS categoria_nombre,
        s.nombre, 
        s.descripcion, 
        s.activo, 
        s.creado_en 
    FROM Academico.subcategorias s
    LEFT JOIN Academico.categorias c ON s.categoria_id = c.id
    WHERE (@id IS NULL OR s.id = @id)
      AND (@categoria_id IS NULL OR s.categoria_id = @categoria_id);
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ActualizarSubcategoria
    @id INT,
    @categoria_id INT,
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Academico.subcategorias
        SET categoria_id = @categoria_id,
            nombre = @nombre,
            descripcion = @descripcion,
            activo = @activo
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La subcategoría especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_EliminarSubcategoria
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Borrado lógico (activo = 0) para mantener consistencia con categorías o físico controlado
        UPDATE Academico.subcategorias
        SET activo = 0
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La subcategoría especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('No se puede eliminar la subcategoría porque está siendo utilizada en otros registros.', 16, 1);
        END
        ELSE
        BEGIN
            ;THROW;
        END
    END CATCH
END;
GO