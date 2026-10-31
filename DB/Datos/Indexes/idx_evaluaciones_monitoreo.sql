USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_entregas_tareas_estudiante')
    CREATE INDEX IX_entregas_tareas_estudiante ON Evaluaciones.entregas_tareas(estudiante_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_entregas_tareas_evaluacion')
    CREATE INDEX IX_entregas_tareas_evaluacion ON Evaluaciones.entregas_tareas(evaluacion_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_progreso_lecciones_estudiante')
    CREATE INDEX IX_progreso_lecciones_estudiante ON Monitoreo.progreso_lecciones(estudiante_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_estudiantes_riesgo_curso')
    CREATE INDEX IX_estudiantes_riesgo_curso ON Monitoreo.estudiantes_riesgo(curso_id, nivel_riesgo);
GO