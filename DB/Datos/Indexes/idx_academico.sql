USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_cursos_subcategoria_id')
    CREATE INDEX IX_cursos_subcategoria_id ON Academico.cursos(subcategoria_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_cursos_docente_id')
    CREATE INDEX IX_cursos_docente_id ON Academico.cursos(docente_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_cursos_codigo_curso')
    CREATE INDEX IX_cursos_codigo_curso ON Academico.cursos(codigo_curso);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_matriculas_estudiante_id')
    CREATE INDEX IX_matriculas_estudiante_id ON Academico.matriculas(estudiante_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_matriculas_curso_id')
    CREATE INDEX IX_matriculas_curso_id ON Academico.matriculas(curso_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_subcategorias_categoria_id')
    CREATE INDEX IX_subcategorias_categoria_id ON Academico.subcategorias(categoria_id);
GO