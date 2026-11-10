USE lms_db;
GO

CREATE PROCEDURE Academico.sp_InsertarCurso
    @subcategoria_id INT,
    @docente_id INT,
    @titulo NVARCHAR(150),
    @descripcion NVARCHAR(MAX),
    @codigo_curso NVARCHAR(20),
    @imagen_portada NVARCHAR(255),
    @estado NVARCHAR(20) = 'Borrador'
AS
BEGIN
    INSERT INTO Academico.cursos (subcategoria_id, docente_id, titulo, descripcion, codigo_curso, imagen_portada, estado)
    VALUES (@subcategoria_id, @docente_id, @titulo, @descripcion, @codigo_curso, @imagen_portada, @estado);
END;
GO

EXEC Academico.sp_InsertarCurso 
    @subcategoria_id = 1, 
    @docente_id = 2, 
    @titulo = 'Matemáticas Básicas', 
    @descripcion = 'Curso introductorio de álgebra y aritmética.', 
    @codigo_curso = 'MAT-101', 
    @imagen_portada = '/img/mat101.jpg', 
    @estado = 'Publicado';


    CREATE PROCEDURE Academico.sp_ConsultarCursos
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, subcategoria_id, docente_id, titulo, descripcion, codigo_curso, imagen_portada, estado, creado_en 
        FROM Academico.cursos;
    END
    ELSE
    BEGIN
        SELECT id, subcategoria_id, docente_id, titulo, descripcion, codigo_curso, imagen_portada, estado, creado_en 
        FROM Academico.cursos 
        WHERE id = @id;
    END
END;
GO



CREATE PROCEDURE Academico.sp_ActualizarCurso
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
    UPDATE Academico.cursos
    SET subcategoria_id = @subcategoria_id,
        docente_id = @docente_id,
        titulo = @titulo,
        descripcion = @descripcion,
        codigo_curso = @codigo_curso,
        imagen_portada = @imagen_portada,
        estado = @estado
    WHERE id = @id;
END;
GO


CREATE PROCEDURE Academico.sp_EliminarCurso
    @id INT
AS
BEGIN
    DELETE FROM Academico.cursos 
    WHERE id = @id;
END;
GO

