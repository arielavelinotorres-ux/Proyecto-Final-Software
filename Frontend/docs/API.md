# API del sistema

El frontend envía solicitudes HTTP al backend ASP.NET. El backend accede a SQL Server en la base existente `lms_db`; no hay acceso SQL desde el navegador.

- Base de rutas: `VITE_API_URL` (por defecto `/api`).
- Autenticación: `Authorization: Bearer <token>`.
- JSON en solicitudes/respuestas, salvo respuestas `204`.
- Errores: `{ "mensaje": "..." }`.
- Roles de la API: `admin`, `docente`, `estudiante`.

## Sesión

| Método | Ruta | Respuesta |
|---|---|---|
| `POST` | `/auth/login` | `{ token, usuario }`; busca la cuenta en `Usuarios.usuarios` y guarda la sesión en `Usuarios.sesiones`. |
| `GET` | `/auth/sesion` | Usuario asociado a una sesión válida. |
| `POST` | `/auth/salir` | `204`; elimina la sesión actual. |

El backend verifica los hashes `pbkdf2-sha256` que genera la aplicación y SHA-256 heredado. No se ofrecen flujos de restablecimiento porque el esquema actual no incluye un procedimiento de restablecimiento/envío.

## Catálogo

| Método | Ruta | Operación |
|---|---|---|
| `GET` | `/categorias` | Lista categorías con `Academico.sp_ConsultarCategorias`. |
| `POST` | `/categorias` | `{ nombre, descripcion, activo }`; administra con `Academico.sp_InsertarCategoria`. |
| `PUT` | `/categorias/{id}` | Actualiza con `Academico.sp_ActualizarCategoria`. |
| `DELETE` | `/categorias/{id}` | Elimina con `Academico.sp_EliminarCategoria`; devuelve `409` si tiene dependencias. |
| `GET` | `/subcategorias` | Lista subcategorías y sus categorías con `Academico.sp_ConsultarSubcategorias`. |
| `POST` | `/subcategorias` | `{ categoriaId, nombre, descripcion, activo }`. |
| `PUT` | `/subcategorias/{id}` | Actualiza la subcategoría. |
| `DELETE` | `/subcategorias/{id}` | Elimina la subcategoría; devuelve `409` si tiene cursos asociados. |

Las operaciones de escritura del catálogo requieren el rol `admin`.

## Usuarios y cursos

| Método | Ruta | Operación |
|---|---|---|
| `GET` | `/usuarios?rol=docente` | Lista usuarios activos para administración; los roles permitidos son `admin`, `docente` y `estudiante`. |
| `GET` | `/dashboard` | Resumen con los cursos visibles al usuario. |
| `GET` | `/cursos` | Cursos globales para administración, propios para docentes y matriculados para estudiantes. |
| `GET` | `/cursos/{id}` | Curso y, para administración/docente, estudiantes matriculados. |
| `POST` | `/cursos` | `{ codigo, nombre, descripcion, docenteId, subcategoriaId, imagenPortada?, estado? }`; usa `Academico.sp_InsertarCurso`. |
| `PUT` | `/cursos/{id}` | Actualiza el curso con `Academico.sp_ActualizarCurso`. |
| `DELETE` | `/cursos/{id}` | Usa `Academico.sp_EliminarCurso`; devuelve `409` si hay dependencias. |
| `POST` | `/cursos/{id}/estudiantes` | `{ estudianteId }`; matricula con `Academico.sp_InsertarMatricula`. |
| `DELETE` | `/cursos/{id}/estudiantes/{estudianteId}` | Elimina la matrícula con `Academico.sp_EliminarMatricula`. |
| `PUT` | `/cursos/{id}/estudiantes/{estudianteId}` | `{ estado: "Activo"|"Completado"|"Cancelado" }`; actualiza con `Academico.sp_ActualizarMatricula`. |
| `GET` | `/cursos/{id}/contenido` | Secciones, lecciones, recursos y estado de progreso del estudiante actual. |
| `PUT` | `/lecciones/{id}/progreso` | `{ completado }`; estudiantes actualizan `Monitoreo.progreso_lecciones`. |

Los identificadores públicos de docentes/estudiantes son los de `Usuarios.usuarios`; el backend los traduce a los IDs de perfil requeridos por los procedimientos de cursos y matrículas.

## Comunicación

| Método | Ruta | Operación |
|---|---|---|
| `GET` | `/cursos/{id}/foros` | Lista foros con `Comunicacion.SP_ListarForos`. |
| `POST` | `/cursos/{id}/foros` | `{ titulo, mensaje }`; `mensaje` se guarda como descripción (máximo 100 caracteres) mediante `Comunicacion.SP_InsertarForo`. Solo el docente del curso crea foros. |
| `PUT` | `/foros/{id}` | `{ titulo, descripcion, estado }`; actualiza con `Comunicacion.SP_ActualizarForo`. |
| `DELETE` | `/foros/{id}` | Archiva con `Comunicacion.SP_DesactivarForo`. |
| `GET` | `/foros/{id}/publicaciones` | Lista publicaciones del foro. |
| `POST` | `/foros/{id}/publicaciones` | `{ contenido, publicacionPadreId? }`; registra una publicación o respuesta. |
| `GET` | `/mensajes` | Bandeja recibida desde `Comunicacion.mensajes_privados`. |
| `POST` | `/mensajes` | `{ receptorId, asunto, contenido }`; guarda en `Comunicacion.mensajes_privados` (contenido máximo 200 caracteres). |
| `PUT` | `/mensajes/{id}/leido` | Marca como leído usando `Comunicacion.SP_ActualizarMensajePrivado`. |
| `GET` | `/contactos` | Lista nombres y roles activos para elegir destinatarios; no devuelve correos. |

Las lecturas de cursos, foros y publicaciones se limitan a cursos accesibles para el usuario.

Los scripts de `SP_ListarMensajesPrivados` y `SP_InsertarMensajePrivado` consultan/escriben una columna `activo` que no existe en la definición de `Comunicacion.mensajes_privados`; por eso el backend usa la tabla tal como está definida y no altera el esquema.

## Límite del esquema actual

La interfaz no expone asistencia, calificaciones, entregas ni evaluaciones porque los scripts SQL disponibles no definen las tablas necesarias para que esas funciones sean persistentes. No se simulan resultados ni se guardan datos de esas funciones en el navegador.
