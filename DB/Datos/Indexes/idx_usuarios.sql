USE lms_db;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_usuarios_correo')
    CREATE INDEX IX_usuarios_correo ON Usuarios.usuarios(correo);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_estudiantes_codigo')
    CREATE INDEX IX_estudiantes_codigo ON Usuarios.estudiantes(codigo_estudiante);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_docentes_codigo')
    CREATE INDEX IX_docentes_codigo ON Usuarios.docentes(codigo_docente);
GO