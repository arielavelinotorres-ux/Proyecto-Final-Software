USE lms_db;
GO

CREATE OR ALTER PROCEDURE Academico.sp_InsertarCategoria
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Academico.categorias (nombre, descripcion, activo)
        OUTPUT INSERTED.id AS categoria_id
        VALUES (@nombre, @descripcion, @activo);
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ConsultarCategorias
    @id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

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

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ActualizarCategoria
    @id INT,
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(255),
    @activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Academico.categorias
        SET nombre = @nombre,
            descripcion = @descripcion,
            activo = @activo
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La categoría especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_EliminarCategoria
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Borrado lógico (activo = 0) para no romper las subcategorías relacionadas
        UPDATE Academico.categorias
        SET activo = 0
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La categoría especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO