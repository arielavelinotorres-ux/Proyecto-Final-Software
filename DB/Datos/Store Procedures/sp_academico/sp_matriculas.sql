USE lms_db;
GO

CREATE PROCEDURE Academico.sp_InsertarMatricula
    @estudiante_id INT,
    @curso_id INT,
    @estado NVARCHAR(20) = 'Activo'
AS
BEGIN
    INSERT INTO Academico.matriculas (estudiante_id, curso_id, estado)
    VALUES (@estudiante_id, @curso_id, @estado);
END;
GO

EXEC Academico.sp_InsertarMatricula 
    @estudiante_id = 5, 
    @curso_id = 1, 
    @estado = 'Activo';


    CREATE PROCEDURE Academico.sp_ConsultarMatriculas
    @id INT = NULL
AS
BEGIN
    IF @id IS NULL
    BEGIN
        SELECT id, estudiante_id, curso_id, fecha_matricula, estado 
        FROM Academico.matriculas;
    END
    ELSE
    BEGIN
        SELECT id, estudiante_id, curso_id, fecha_matricula, estado 
        FROM Academico.matriculas 
        WHERE id = @id;
    END
END;
GO


CREATE PROCEDURE Academico.sp_ActualizarMatricula
    @id INT,
    @estudiante_id INT,
    @curso_id INT,
    @estado NVARCHAR(20)
AS
BEGIN
    UPDATE Academico.matriculas
    SET estudiante_id = @estudiante_id,
        curso_id = @curso_id,
        estado = @estado
    WHERE id = @id;
END;
GO

EXEC Academico.sp_ActualizarMatricula 
    @id = 1, 
    @estudiante_id = 5, 
    @curso_id = 1, 
    @estado = 'Completado';


    CREATE PROCEDURE Academico.sp_EliminarMatricula
    @id INT
AS
BEGIN
    DELETE FROM Academico.matriculas 
    WHERE id = @id;
END;
GO