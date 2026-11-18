USE lms_db;
GO


CREATE OR ALTER PROCEDURE Academico.sp_InsertarMatricula
    @estudiante_id INT,
    @curso_id INT,
    @estado NVARCHAR(20) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el estudiante no esté matriculado dos veces en el mismo curso
    IF EXISTS (SELECT 1 FROM Academico.matriculas WHERE estudiante_id = @estudiante_id AND curso_id = @curso_id)
    BEGIN
        RAISERROR('El estudiante ya se encuentra matriculado en este curso.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Academico.matriculas (estudiante_id, curso_id, estado)
        OUTPUT INSERTED.id AS matricula_id
        VALUES (@estudiante_id, @curso_id, @estado);
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            RAISERROR('El estudiante ya se encuentra matriculado en este curso.', 16, 1);
            RETURN;
        END

        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ConsultarMatriculas
    @id INT = NULL,
    @estudiante_id INT = NULL,
    @curso_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        m.id, 
        m.estudiante_id, 
        CONCAT(e.nombres, ' ', e.apellidos) AS estudiante_nombre,
        m.curso_id, 
        c.titulo AS curso_titulo,
        m.fecha_matricula, 
        m.estado 
    FROM Academico.matriculas m
    LEFT JOIN Academico.usuarios e ON m.estudiante_id = e.id
    LEFT JOIN Academico.cursos c ON m.curso_id = c.id
    WHERE (@id IS NULL OR m.id = @id)
      AND (@estudiante_id IS NULL OR m.estudiante_id = @estudiante_id)
      AND (@curso_id IS NULL OR m.curso_id = @curso_id);
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ActualizarMatricula
    @id INT,
    @estudiante_id INT,
    @curso_id INT,
    @estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar duplicidad excluyendo el registro actual
    IF EXISTS (SELECT 1 FROM Academico.matriculas WHERE estudiante_id = @estudiante_id AND curso_id = @curso_id AND id <> @id)
    BEGIN
        RAISERROR('El estudiante ya se encuentra matriculado en este curso.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        UPDATE Academico.matriculas
        SET estudiante_id = @estudiante_id,
            curso_id = @curso_id,
            estado = @estado
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La matrícula especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            RAISERROR('El estudiante ya se encuentra matriculado en este curso.', 16, 1);
            RETURN;
        END

        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_EliminarMatricula
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Academico.matriculas 
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('La matrícula especificada no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('No se puede eliminar la matrícula porque está siendo utilizada en otros registros.', 16, 1);
        END
        ELSE
        BEGIN
            ;THROW;
        END
    END CATCH
END;
GO