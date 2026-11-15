USE lms_db
GO

-- SP para Registrar o sumar un intento fallido - MODIFICADO

CREATE OR ALTER PROCEDURE Usuarios.SP_RegistrarIntentoFallido
    @correo NVARCHAR(150),
    @ip_origen NVARCHAR(45),
    @bloqueado_hasta DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Usuarios.intentos_fallidos WHERE correo = @correo)
        BEGIN
            UPDATE Usuarios.intentos_fallidos
            SET intentos = intentos + 1, ip_origen = @ip_origen, bloqueado_hasta = @bloqueado_hasta
            WHERE correo = @correo;
        END
        ELSE
        BEGIN
            INSERT INTO Usuarios.intentos_fallidos (correo, ip_origen, intentos, bloqueado_hasta)
            VALUES (@correo, @ip_origen, 1, @bloqueado_hasta);
        END
        
        -- Devuelve estado actual a C#
        SELECT intentos, bloqueado_hasta FROM Usuarios.intentos_fallidos WHERE correo = @correo;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al registrar intento. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO


-- SP para Limpiar intentos (cuando el usuario hace login con éxito) - MODIFICADO

CREATE OR ALTER PROCEDURE Usuarios.SP_LimpiarIntentos
    @correo NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Usuarios.intentos_fallidos WHERE correo = @correo;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al limpiar intentos. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO