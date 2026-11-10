USE lms_db
GO
-- 1. 
CREATE PROCEDURE Configuracion.sp_InsertarParametroSistema
    @clave NVARCHAR(100),
    @valor NVARCHAR(MAX),
    @descripcion NVARCHAR(255) = NULL,
    @tipo_dato NVARCHAR(20) = 'Texto'
AS
BEGIN
    INSERT INTO Configuracion.parametros_sistema (clave, valor, descripcion, tipo_dato)
    VALUES (@clave, @valor, @descripcion, @tipo_dato);
END;
GO

-- 2. 
CREATE PROCEDURE Configuracion.sp_ConsultarParametrosSistema
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, clave, valor, descripcion, tipo_dato, actualizado_en 
        FROM Configuracion.parametros_sistema;
    END
    ELSE
    BEGIN
        SELECT id, clave, valor, descripcion, tipo_dato, actualizado_en 
        FROM Configuracion.parametros_sistema 
        WHERE id = @id;
    END
END;
GO

-- 3. 
CREATE PROCEDURE Configuracion.sp_ActualizarParametroSistema
    @id INT,
    @clave NVARCHAR(100),
    @valor NVARCHAR(MAX),
    @descripcion NVARCHAR(255) = NULL,
    @tipo_dato NVARCHAR(20)
AS
BEGIN
    UPDATE Configuracion.parametros_sistema
    SET clave = @clave,
        valor = @valor,
        descripcion = @descripcion,
        tipo_dato = @tipo_dato,
        actualizado_en = GETDATE() -- Actualiza la marca de tiempo automáticamente
    WHERE id = @id;
END;
GO

-- 4. 
CREATE PROCEDURE Configuracion.sp_EliminarParametroSistema
    @id INT
AS
BEGIN
    DELETE FROM Configuracion.parametros_sistema 
    WHERE id = @id;
END;
GO