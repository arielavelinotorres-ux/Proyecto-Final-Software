# HomeClass API

API ASP.NET Web API para el frontend React. El backend usa la base `lms_db` y los esquemas SQL existentes (`Usuarios`, `Academico`, `Contenido` y `Comunicacion`); el frontend no se conecta directamente a SQL Server.

## Requisitos

- Visual Studio 2022 con ASP.NET y Web Developer tools.
- .NET Framework 4.8.
- La base `lms_db` ya creada y con sus tablas y procedimientos disponibles en SQL Server.

## Conexión

La cadena de conexión se configura en `Web.config`, bajo `HomeClassDb`. El valor del repositorio usa SQL Server local (`Data Source=.`), autenticación integrada y la base existente `lms_db`. Si la instancia es distinta, cambia solo el host o el método de autenticación en la configuración local del backend. No se guarda ninguna credencial en el frontend.

La API invoca los procedimientos existentes de categorías, subcategorías, cursos, matrículas y foros. También consulta las tablas de contenido, usuarios, sesiones, foros, publicaciones y mensajes ya existentes. Los procedimientos de listar/insertar mensajes hacen referencia a una columna `activo` ausente en la tabla de mensajes, así que el backend evita esas dos rutinas y opera contra la tabla existente.

## Ejecutar

1. Verifica que SQL Server tenga la base existente `lms_db`, con sus tablas, roles, perfiles y procedimientos ya creados. La aplicación no ejecuta DDL ni cambia el esquema; sus operaciones modifican únicamente los datos necesarios para las funciones expuestas.
2. Configura `HomeClassDb` en `Web.config` para apuntar a esa instancia y concede al usuario del proceso de IIS permiso de lectura/escritura sobre las operaciones utilizadas.
3. Abre `HomeClassApi.csproj` en Visual Studio, selecciona el proyecto como inicio y presiona `F5`. La API queda en `http://localhost:5000`.
4. En `Frontend`, copia `.env.example` como `.env` si necesitas cambiar el proxy y ejecuta `npm ci` y `npm run dev`.

## Funcionalidad conectada

- Inicio de sesión y sesiones persistidas en `Usuarios.usuarios`, `Usuarios.roles` y `Usuarios.sesiones`.
- Catálogo de categorías/subcategorías.
- Cursos y matrículas, usando los procedimientos almacenados de `Academico`.
- Lectura de secciones y lecciones.
- Recursos asociados a lecciones y progreso persistido por estudiante.
- Foros, publicaciones y mensajes privados, usando los objetos de `Comunicacion`.

La app limita su navegación a las funciones representadas en el esquema disponible. Los scripts actuales no definen una tabla de asistencia ni una entidad principal de evaluaciones/calificaciones; esas pantallas no se ofrecen. El login acepta hashes PBKDF2 con prefijo `pbkdf2-sha256` y hashes SHA-256 heredados.
