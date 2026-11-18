USE lms_db
GO



-- 1. Insertar Registro de Auditoría
CREATE OR ALTER PROCEDURE Configuracion.sp_InsertarAuditoriaConfiguracion
    @parametro_id INT,
    @usuario_id INT,
    @valor_anterior NVARCHAR(MAX) = NULL,
    @valor_nuevo NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Configuracion.auditoria_configuraciones (parametro_id, usuario_id, valor_anterior, valor_nuevo)
        OUTPUT INSERTED.id AS auditoria_id
        VALUES (@parametro_id, @usuario_id, @valor_anterior, @valor_nuevo);
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END;
GO

--------------------------------------------------------------------

-- 2. Consultar Registros de Auditoría (con JOINS y ordenados del más reciente al más antiguo)
CREATE OR ALTER PROCEDURE Configuracion.sp_ConsultarAuditoriaConfiguraciones
    @id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.id, 
        a.parametro_id, 
        p.clave AS parametro_clave,
        a.usuario_id, 
        CONCAT(u.nombres, ' ', u.apellidos) AS usuario_nombre, -- Ajustar según los campos reales de la tabla de usuarios
        a.valor_anterior, 
        a.valor_nuevo, 
        a.fecha_cambio 
    FROM Configuracion.auditoria_configuraciones a
    LEFT JOIN Configuracion.parametros_sistema p ON a.parametro_id = p.id
    LEFT JOIN Usuarios.usuarios u ON a.usuario_id = u.id
    WHERE (@id IS NULL OR a.id = @id)
    ORDER BY a.fecha_cambio DESC;
END;
GO