USE lms_db
GO

-- SP para listar Roles Activos
CREATE OR ALTER PROCEDURE Usuarios.SP_ListarRoles
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT id, nombre
        FROM Usuarios.roles
        ORDER BY id ASC;
    END TRY
    BEGIN CATCH
        DECLARE @DetalleError NVARCHAR(4000) = ERROR_MESSAGE();
        ;THROW 50002, 'Falló al listar los roles.', 1;
    END CATCH
END;
GO

