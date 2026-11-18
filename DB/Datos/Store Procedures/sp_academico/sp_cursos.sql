USE lms_db;
GO

CREATE OR ALTER PROCEDURE Academico.sp_InsertarCurso
    @subcategoria_id INT,
    @docente_id INT,
    @titulo NVARCHAR(150),
    @descripcion NVARCHAR(MAX),
    @codigo_curso NVARCHAR(20),
    @imagen_portada NVARCHAR(255),
    @estado NVARCHAR(20) = 'Borrador'
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos vacíos o nulos para título y código de curso
    IF LTRIM(RTRIM(ISNULL(@titulo, ''))) = '' OR LTRIM(RTRIM(ISNULL(@codigo_curso, ''))) = ''
    BEGIN
        RAISERROR('El título y el código del curso son obligatorios y no pueden estar vacíos.', 16, 1);
        RETURN;
    END

    -- Validar si el código del curso ya existe
    IF EXISTS (SELECT 1 FROM Academico.cursos WHERE codigo_curso = @codigo_curso)
    BEGIN
        RAISERROR('El código del curso ya se encuentra registrado.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Academico.cursos (subcategoria_id, docente_id, titulo, descripcion, codigo_curso, imagen_portada, estado)
        OUTPUT INSERTED.id AS curso_id
        VALUES (@subcategoria_id, @docente_id, @titulo, @descripcion, @codigo_curso, @imagen_portada, @estado);
    END TRY
    BEGIN CATCH
        -- Capturar errores por restricciones únicas o duplicadas a nivel de BD
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            RAISERROR('El código del curso ya se encuentra registrado.', 16, 1);
            RETURN;
        END
        
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ConsultarCursos
    @id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @id IS NULL
    BEGIN
        SELECT 
            c.id, 
            c.subcategoria_id, 
            s.nombre AS subcategoria_nombre,
            c.docente_id, 
            CONCAT(d.nombres, ' ', d.apellidos) AS docente_nombre, -- Ajustar según los campos de tu tabla docentes (ej. nombre/apellido)
            c.titulo, 
            c.descripcion, 
            c.codigo_curso, 
            c.imagen_portada, 
            c.estado, 
            c.creado_en 
        FROM Academico.cursos c
        LEFT JOIN Academico.subcategorias s ON c.subcategoria_id = s.id
        LEFT JOIN Academico.docentes d ON c.docente_id = d.id;
    END
    ELSE
    BEGIN
        SELECT 
            c.id, 
            c.subcategoria_id, 
            s.nombre AS subcategoria_nombre,
            c.docente_id, 
            CONCAT(d.nombres, ' ', d.apellidos) AS docente_nombre,
            c.titulo, 
            c.descripcion, 
            c.codigo_curso, 
            c.imagen_portada, 
            c.estado, 
            c.creado_en 
        FROM Academico.cursos c
        LEFT JOIN Academico.subcategorias s ON c.subcategoria_id = s.id
        LEFT JOIN Academico.docentes d ON c.docente_id = d.id
        WHERE c.id = @id;
    END
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_ActualizarCurso
    @id INT,
    @subcategoria_id INT,
    @docente_id INT,
    @titulo NVARCHAR(150),
    @descripcion NVARCHAR(MAX),
    @codigo_curso NVARCHAR(20),
    @imagen_portada NVARCHAR(255),
    @estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos vacíos o nulos
    IF LTRIM(RTRIM(ISNULL(@titulo, ''))) = '' OR LTRIM(RTRIM(ISNULL(@codigo_curso, ''))) = ''
    BEGIN
        RAISERROR('El título y el código del curso son obligatorios y no pueden estar vacíos.', 16, 1);
        RETURN;
    END

    -- Validar si el código del curso ya pertenece a otro registro
    IF EXISTS (SELECT 1 FROM Academico.cursos WHERE codigo_curso = @codigo_curso AND id <> @id)
    BEGIN
        RAISERROR('El código del curso ya se encuentra registrado en otro curso.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        UPDATE Academico.cursos
        SET subcategoria_id = @subcategoria_id,
            docente_id = @docente_id,
            titulo = @titulo,
            descripcion = @descripcion,
            codigo_curso = @codigo_curso,
            imagen_portada = @imagen_portada,
            estado = @estado
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('El curso especificado no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            RAISERROR('El código del curso ya se encuentra registrado.', 16, 1);
            RETURN;
        END

        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

CREATE OR ALTER PROCEDURE Academico.sp_EliminarCurso
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM Academico.cursos 
        WHERE id = @id;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('El curso especificado no existe.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        -- Captura típica si hay relación con llaves foráneas
        IF ERROR_NUMBER() = 547
        BEGIN
            RAISERROR('No se puede eliminar el curso porque está siendo utilizado en otros registros.', 16, 1);
        END
        ELSE
        BEGIN
            ;THROW;
        END
    END CATCH
END;
GO