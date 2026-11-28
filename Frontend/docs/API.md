# Integración frontend/API

El frontend consume la Minimal API ASP.NET mediante `fetch`; solo el backend accede a SQL Server. La capa HTTP central está en [`src/api/http.js`](../src/api/http.js).

## Configuración de conexión

- El prefijo por defecto es `VITE_API_URL=/api` (archivo [`src/config.js`](../src/config.js)).
- En desarrollo, Vite reenvía `/api` al `VITE_PROXY` configurado en [`vite.config.js`](../vite.config.js). El valor predeterminado es `http://localhost:5296`, puerto del perfil HTTP en `Backend/Proyecto/SistemaEducativoDeRiesgo/Properties/launchSettings.json`.
- Para iniciar el backend: desde su proyecto, ejecutar `dotnet run --launch-profile http`; para el frontend, ejecutar `npm run dev` dentro de `Frontend`.
- En producción, el frontend debe servirse bajo el mismo origen que la API para conservar `/api`. El backend actual no configura CORS; no se debe apuntar el navegador a un host separado esperando que la conexión funcione.
- Las solicitudes y respuestas usan JSON. El cliente acepta respuestas vacías `204` y errores JSON con `mensaje`, `detail` o `title`.

## Contratos publicados por el backend actual

Las rutas siguientes llevan el prefijo `/api`.

### Usuarios y credenciales

| Método | Ruta | Solicitud/respuesta |
|---|---|---|
| `POST` | `/usuarios/login` | `{ correo, password }` → `{ id, rolId, rolNombre, correo, activo }`; credenciales inválidas/inactivas devuelven `401` y `{ mensaje }`. |
| `GET` | `/usuarios/` | Lista `{ id, rolId, rolNombre, correo, activo, creadoEn, ultimoAcceso }`. |
| `POST` | `/usuarios/` | `{ rolId, correo, password }` → `201` con `{ id }`. |
| `PUT` | `/usuarios/{id}/perfil` | `{ rolId, correo }`. |
| `PUT` | `/usuarios/{id}/password` | `{ nuevoPassword }`. |
| `PATCH` | `/usuarios/{id}/activar` | Activa la cuenta. |
| `PATCH` | `/usuarios/{id}/desactivar` | Desactiva la cuenta. |
| `POST` | `/usuarios/{id}/cerrar-sesiones` | Cierra las sesiones del usuario almacenadas por el backend. |

Los perfiles relacionados están disponibles en `GET/POST /docentes/`, `PUT/DELETE /docentes/{id}` y `GET/POST /estudiantes/`, `GET /estudiantes/usuario/{usuarioId}`, `PUT/DELETE /estudiantes/{id}`. Los DTO de creación/actualización se detallan en `Backend/Proyecto/SistemaEducativoDeRiesgo.Application/DTOs/Usuarios/DocenteDtos.cs` y `EstudianteDtos.cs`.

Recuperación de contraseña: `POST /recuperacion-password/generar` con `{ usuarioId, minutosExpiracion? }`; `POST /recuperacion-password/validar` con `{ token }`; `PATCH /recuperacion-password/{id}/usar`. No hay flujo de entrega del token por correo en la interfaz.

El login no entrega token, cookie ni identificador de sesión. El frontend adapta `rolNombre` (Administrador/Docente/Estudiante) a los valores usados por su interfaz y conserva el perfil en `sessionStorage` hasta cerrar la pestaña o salir. **Este estado local no autentica ni autoriza solicitudes posteriores.**

### Comunicación

| Método | Ruta | Solicitud/respuesta |
|---|---|---|
| `POST` | `/mensajes-privados/` | `{ emisorId, receptorId, asunto, contenido }` → `201` con `{ id }`. |
| `GET` | `/mensajes-privados/recibidos/{receptorId}` | Lista `{ id, emisorId, emisorNombre, receptorId, asunto, contenido, leido, fechaEnvio }`. |
| `GET` | `/mensajes-privados/enviados/{emisorId}` | Lista los mensajes enviados por ese usuario. |
| `PATCH` | `/mensajes-privados/{id}/leer` | Marca como leído. |
| `DELETE` | `/mensajes-privados/{id}` | Elimina un mensaje. |
| `GET` | `/foros/curso/{cursoId}` | Lista foros del curso. |
| `POST` | `/foros/` | `{ cursoId, titulo, descripcion }` → `201` con `{ id }`. |
| `PUT` | `/foros/{id}` | `{ titulo, descripcion, estado }`. |
| `PATCH` | `/foros/{id}/archivar` | Archiva el foro. |
| `GET` | `/publicaciones-foro/foro/{foroId}` | Lista publicaciones del foro. |
| `POST` | `/publicaciones-foro/` | `{ foroId, usuarioId, publicacionPadreId, contenido }` → `201` con `{ id }`. |
| `PUT` | `/publicaciones-foro/{id}` | `{ contenido }`. |
| `DELETE` | `/publicaciones-foro/{id}` | Elimina una publicación. |
| `GET` | `/notificaciones/usuario/{usuarioId}` | Lista notificaciones del usuario. |
| `PATCH` | `/notificaciones/{id}/leer` | Marca una notificación como leída. |
| `PATCH` | `/notificaciones/usuario/{usuarioId}/leer-todas` | Marca todas como leídas. |
| `POST` | `/notificaciones/` | `{ usuarioId, titulo, mensaje }` → `201` con `{ id }`. |
| `DELETE` | `/notificaciones/{id}` | Elimina una notificación. |

La pantalla de mensajes usa `id` y `rolNombre` de `/usuarios/` para destinatarios, pasa explícitamente emisor/receptor al crear un mensaje y consume los nombres reales de las propiedades camelCase devueltas por la API.

## Límites deliberados y seguridad

- El backend actual **no publica endpoints de cursos, dashboard ni categorías/subcategorías**. Por eso esas vistas no están conectadas ni aparecen en el panel activo; las pantallas heredadas quedan fuera de navegación.
- La API tampoco tiene middleware/policies de autorización visibles en `Program.cs` ni aplica autenticación en estos endpoints. En particular, el cliente puede modificar el usuario guardado y enviar otros `emisorId`/IDs. La protección de rutas del frontend es solo UX, no control de acceso.
- No desplegar datos reales ni considerar seguro este flujo hasta que el backend autentique solicitudes y derive el usuario/rol de una credencial validada. Hacerlo solo en el frontend no corrige la vulnerabilidad.
- La API actual no configura CORS. No se debe resolver cambiando a una URL de API cross-origin desde el navegador; requiere mismo origen o configuración del backend (fuera del alcance solicitado).
- Los endpoints de cursos, asistencia, calificaciones, evaluaciones, entregas y materiales no están en esta API; no se generan respuestas simuladas.
