# HomeClass API

Backend ASP.NET Web API para la app del frontend `Frontend`.

## Qué contiene

- Autenticación por token Bearer y sesiones activas verificadas contra SQL Server.
- Endpoints del contrato `Frontend/docs/API.md`: usuarios, perfil, cursos, evaluaciones, notas, asistencia, materiales y entregas.
- Archivos almacenados fuera del sitio web, con identificadores aleatorios y descargas autorizadas.
- Datos académicos de demostración creados por `DB/Base web.sql`.

## Requisitos

- Visual Studio 2022 con ASP.NET y Web Developer tools.
- .NET Framework 4.8.

## Ejecutar

1. Ejecutar `DB/Base web.sql` en una instancia local de SQL Server.
2. Ejecutar `DB/script/seguridad-cuentas.sql` en esa misma base para habilitar el bloqueo después de cinco intentos fallidos, solicitudes de restablecimiento y perfiles de administración por módulo.
3. Si la instancia no es `localhost`/`.`, ajustar la cadena `HomeClassDb` de `Web.config`.
4. Abrir el proyecto en Visual Studio, seleccionar `HomeClassApi` como inicio y presionar `F5`.
5. El backend queda en `http://localhost:5000`; el frontend lo consume mediante el proxy de Vite.

## Credenciales demo

- Admin: `admin@homeclass.com` / `admin123`
- Docente: `docente@homeclass.com` / `docente123`
- Estudiante: `estudiante@homeclass.com` / `estudiante123`

La carpeta de archivos se configura mediante `HomeClassUploadPath`; si se omite, se usa `%LOCALAPPDATA%\HomeClass\Uploads`.
