USE lms_db;
GO

CREATE OR ALTER VIEW Evaluaciones.vw_resumen_evaluaciones AS
SELECT 
    et.id AS entrega_id,
    e.id AS evaluacion_id,
    e.titulo AS titulo_evaluacion,
    e.tipo AS tipo_evaluacion,
    c.id AS curso_id,
    c.titulo AS titulo_curso,
    est.id AS estudiante_id,
    CONCAT(est.nombre, ' ', est.apellido) AS nombre_estudiante,
    et.fecha_entrega,
    et.calificacion,
    et.estado AS estado_entrega
FROM Evaluaciones.entregas_tareas et
INNER JOIN Evaluaciones.evaluaciones e ON et.evaluacion_id = e.id
INNER JOIN Academico.cursos c ON e.curso_id = c.id
INNER JOIN Usuarios.estudiantes est ON et.estudiante_id = est.id;
GO