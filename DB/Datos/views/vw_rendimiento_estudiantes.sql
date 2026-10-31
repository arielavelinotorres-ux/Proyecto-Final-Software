USE lms_db;
GO

CREATE OR ALTER VIEW Academico.vw_rendimiento_estudiantes AS
SELECT 
    m.id AS matricula_id,
    e.id AS estudiante_id,
    e.codigo_estudiante,
    CONCAT(e.nombre, ' ', e.apellido) AS nombre_completo_estudiante,
    c.id AS curso_id,
    c.titulo AS curso_titulo,
    m.fecha_matricula,
    m.estado AS estado_matricula
FROM Academico.matriculas m
INNER JOIN Usuarios.estudiantes e ON m.estudiante_id = e.id
INNER JOIN Academico.cursos c ON m.curso_id = c.id;
GO