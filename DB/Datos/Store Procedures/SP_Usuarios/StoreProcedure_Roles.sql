USE lms_db
GO

-- SP para listar Roles Activos - MODIFICADO

CREATE OR ALTER PROCEDURE Usuarios.SP_ListarRoles
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT id, nombre FROM Usuarios.roles ORDER BY id ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @MensajeFinal NVARCHAR(4000) = 'Falló al listar roles. Detalle: ' + @DetalleError;
        ;THROW 50002, @MensajeFinal, 1;
    END CATCH
END;
GO

