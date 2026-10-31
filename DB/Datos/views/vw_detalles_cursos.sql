USE lms_db;
GO

CREATE OR ALTER VIEW Academico.vw_detalles_cursos AS
SELECT 
    c.id AS curso_id,
    c.codigo_curso,
    c.titulo AS curso_titulo,
    c.descripcion AS curso_descripcion,
    c.estado AS curso_estado,
    c.imagen_portada,
    cat.nombre AS categoria,
    sub.nombre AS subcategoria,
    CONCAT(d.nombre, ' ', d.apellido) AS nombre_docente,
    d.codigo_docente,
    c.creado_en
FROM Academico.cursos c
INNER JOIN Academico.subcategorias sub ON c.subcategoria_id = sub.id
INNER JOIN Academico.categorias cat ON sub.categoria_id = cat.id
INNER JOIN Usuarios.docentes d ON c.docente_id = d.id;
GO