USE lms_db;
GO

CREATE OR ALTER TRIGGER Configuracion.trg_auditoria_cambios_parametros
ON Configuracion.parametros_sistema
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Registrar el cambio solo si la columna 'valor' fue modificada
    IF UPDATE(valor)
    BEGIN
        INSERT INTO Configuracion.auditoria_configuraciones (
            parametro_id,
            usuario_id,
            valor_anterior,
            valor_nuevo,
            fecha_cambio
        )
        SELECT 
            i.id,
            1, -- ID de usuario por defecto/sistema (modificar si se pasa por contexto de sesión)
            d.valor AS valor_anterior,
            i.valor AS valor_nuevo,
            GETDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.id = d.id
        WHERE i.valor <> d.valor;
    END
END;
GO