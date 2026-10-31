USE lms_db;
GO

CREATE OR ALTER TRIGGER Monitoreo.trg_actualizar_progreso_leccion
ON Monitoreo.progreso_lecciones
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Solo actúa si la columna 'completado' cambió a 1 (Verdadero)
    IF UPDATE(completado)
    BEGIN
        UPDATE pl
        SET fecha_completado = GETDATE()
        FROM Monitoreo.progreso_lecciones pl
        INNER JOIN inserted i ON pl.id = i.id
        WHERE i.completado = 1 AND i.fecha_completado IS NULL;
    END
END;
GO