USE lms_db
GO

--1
CREATE PROCEDURE Configuracion.sp_InsertarAuditoriaConfiguracion
    @parametro_id INT,
    @usuario_id INT,
    @valor_anterior NVARCHAR(MAX) = NULL,
    @valor_nuevo NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Configuracion.auditoria_configuraciones (parametro_id, usuario_id, valor_anterior, valor_nuevo)
    VALUES (@parametro_id, @usuario_id, @valor_anterior, @valor_nuevo);
END;
GO

-- 2.
CREATE PROCEDURE Configuracion.sp_ConsultarAuditoriaConfiguraciones
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, parametro_id, usuario_id, valor_anterior, valor_nuevo, fecha_cambio 
        FROM Configuracion.auditoria_configuraciones;
    END
    ELSE
    BEGIN
        SELECT id, parametro_id, usuario_id, valor_anterior, valor_nuevo, fecha_cambio 
        FROM Configuracion.auditoria_configuraciones 
        WHERE id = @id;
    END
END;
GO

-- 3. 
CREATE PROCEDURE Configuracion.sp_ActualizarAuditoriaConfiguracion
    @id INT,
    @parametro_id INT,
    @usuario_id INT,
    @valor_anterior NVARCHAR(MAX) = NULL,
    @valor_nuevo NVARCHAR(MAX)
AS
BEGIN
    UPDATE Configuracion.auditoria_configuraciones
    SET parametro_id = @parametro_id,
        usuario_id = @usuario_id,
        valor_anterior = @valor_anterior,
        valor_nuevo = @valor_nuevo
    WHERE id = @id;
END;
GO

-- 4. 
CREATE PROCEDURE Configuracion.sp_EliminarAuditoriaConfiguracion
    @id INT
AS
BEGIN
    DELETE FROM Configuracion.auditoria_configuraciones 
    WHERE id = @id;
END;
GO