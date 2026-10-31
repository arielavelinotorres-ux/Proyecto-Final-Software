USE lms_db;
GO

CREATE OR ALTER TRIGGER Evaluaciones.trg_alerta_estudiante_riesgo
ON Evaluaciones.entregas_tareas
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(calificacion)
    BEGIN
        INSERT INTO Monitoreo.estudiantes_riesgo (
            estudiante_id,
            curso_id,
            nivel_riesgo,
            motivo,
            detectado_en,
            atendido
        )
        SELECT 
            i.estudiante_id,
            ev.curso_id,
            'Alto',
            CONCAT('Calificación crítica (', i.calificacion, ') en la evaluación: ', ev.titulo),
            GETDATE(),
            0
        FROM inserted i
        INNER JOIN Evaluaciones.evaluaciones ev ON i.evaluacion_id = ev.id
        WHERE i.calificacion IS NOT NULL 
          AND i.calificacion < 60.00
          AND NOT EXISTS (
              SELECT 1 
              FROM Monitoreo.estudiantes_riesgo er
              WHERE er.estudiante_id = i.estudiante_id 
                AND er.curso_id = ev.curso_id 
                AND er.atendido = 0
          );
    END
END;
GO