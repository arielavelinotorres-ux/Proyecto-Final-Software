# Contrato de la API (backend ↔ frontend)

Todo es JSON salvo las subidas (`multipart/form-data`) y las descargas (archivo binario).
Base: `VITE_API_URL` (por defecto `/api`). Autenticación: `Authorization: Bearer <token>`.

**Errores:** código HTTP adecuado (400 validación, 401 sin sesión, 403 sin permiso, 404, 409 conflicto) y cuerpo
`{ "mensaje": "Texto en español para mostrar al usuario" }`. El frontend muestra ese texto tal cual.

**Fechas:** `fecha` = `"YYYY-MM-DD"`; `fechaHora` = ISO 8601. **Roles:** `"admin" | "docente" | "estudiante"`.
**Estados de riesgo:** `"alto" | "moderado" | "bajo" | "sin-datos"`.

## Autenticación

| Método y ruta | Cuerpo | Respuesta |
|---|---|---|
| `POST /auth/login` | `{ correo, clave }` | `{ token, usuario: { id, nombre, correo, rol, activo } }` (401 si falla; 403 si cuenta inactiva) |
| `POST /auth/solicitar-restablecimiento` | `{ correo }` | Mensaje genérico; registra la solicitud solo para cuentas activas de estudiantes/docentes. El quinto intento fallido también bloquea y genera solicitud. |
| `GET /auth/sesion` | | `usuario` (401 si el token no sirve) |

## Dashboard — `GET /dashboard` (respuesta según el rol de quien pregunta)

- **admin:** `{ rol, usuarios: { total, admins, docentes, estudiantes, inactivos }, cursos: [CursoResumen], promedioGeneral, asistenciaGeneral, riesgo: { alto, moderado, bajo } }`
- **docente:** `{ rol, cursos: [CursoResumen], porRevisar, enRiesgoAlto, promedioGeneral, alertas: [{ estudianteId, nombre, cursoId, curso, riesgo, promedio, asistencia }], proximas: [{ evaluacionId, cursoId, curso, nombre, tipo, fecha }] }`
  (`porRevisar` = entregas sin nota; `alertas` = riesgo alto o moderado, máx. 8; `proximas` = evaluaciones desde hoy, máx. 6)
- **estudiante:** `{ rol, promedio, asistencia, cursos: [{ id, nombre, promedio, promedioGrupo, asistencia, riesgo }], pendientes: [{ evaluacionId, cursoId, curso, nombre, tipo, fecha, vencida }] }`
  (`pendientes` = evaluaciones que piden archivo, sin entrega y sin nota)

`CursoResumen = { id, codigo, nombre, periodo, docenteId, docente, estudiantes, promedio, asistencia, enRiesgo }` (`enRiesgo` = estudiantes con riesgo alto).

## Usuarios (solo admin)

| Método y ruta | Cuerpo | Respuesta |
|---|---|---|
| `GET /usuarios` | | `[usuario]` |
| `POST /usuarios` | `{ nombre, correo, rol, clave }` (clave ≥ 8) | `usuario` · 409/400 si el correo ya existe |
| `PUT /usuarios/{id}` | `{ nombre, correo, rol, activo }` | `usuario` · no puede desactivarse ni cambiar su propio rol |
| `DELETE /usuarios/{id}` | | 204 · no puede borrarse a sí mismo; un docente con cursos no se borra |
| `POST /usuarios/{id}/restablecer-clave` | `{ nueva }` (clave ≥ 8) | 204 · solo admin; restablece y desbloquea una cuenta docente/estudiante |

Los usuarios con rol `admin` tienen `tipoAdmin`: `general`, `usuarios`, `cursos` o `reportes`. `permisosAdmin` informa los módulos autorizados; solo administración general puede asignar perfiles administrativos.

## Reportes y comunicación

| Método y ruta | Quién | Respuesta |
|---|---|---|
| `GET /admin/auditoria?limite=100` | admin | eventos recientes: fecha, usuario, acción, entidad y detalle (máximo 300) |
| `GET /cursos/{id}/avisos` | con acceso al curso | avisos del curso, del más reciente al más antiguo |
| `POST /cursos/{id}/avisos` | docente asignado | `{ titulo, mensaje }` → aviso publicado |
| `GET /agenda` | docente/estudiante | evaluaciones y avisos de los cursos propios o matriculados |
| `GET /estudiante/historial` | estudiante | evaluaciones, notas, estados y entregas de sus cursos |

## Perfil (cualquier rol)

| `PUT /perfil` | `{ nombre }` | `usuario` |
|---|---|---|
| `PUT /perfil/clave` | `{ actual, nueva }` (nueva ≥ 8) | 204 |

## Cursos

| Método y ruta | Quién | Cuerpo / Respuesta |
|---|---|---|
| `GET /cursos` | todos | `[CursoResumen]` filtrado: admin todos · docente los suyos · estudiante los matriculados (con `promedio`, `asistencia` y `riesgo` **del estudiante**) |
| `GET /cursos/{id}` | con acceso | `{ id, codigo, nombre, periodo, descripcion, docenteId, docente, estudiantes: [{ id, nombre, correo }] }` (`estudiantes` vacío para el rol estudiante) |
| `POST /cursos` | admin | `{ codigo, nombre, periodo, descripcion, docenteId }` → curso · código único |
| `PUT /cursos/{id}` | admin | igual que POST |
| `DELETE /cursos/{id}` | admin | 204 · borra en cascada evaluaciones, notas, asistencia, entregas y **sus archivos** |
| `POST /cursos/{id}/estudiantes` | admin | `{ estudianteId }` → 204 |
| `DELETE /cursos/{id}/estudiantes/{estudianteId}` | admin | 204 · borra sus notas, asistencia y entregas en el curso |
| `GET /cursos/{id}/mi-curso` | estudiante | ver abajo |

`GET /cursos/{id}/mi-curso` →
`{ curso: { id, codigo, nombre, descripcion, periodo, docente }, promedio, promedioGrupo, asistencia, riesgo, evaluaciones: [Evaluacion + { nota, estado, entrega }] }`
donde `estado` = `"calificada" | "entregada" | "vencida" | "pendiente"` y `entrega` = `Entrega | null`.

## Evaluaciones (docente del curso)

`Evaluacion = { id, cursoId, nombre, tipo: "tarea"|"proyecto"|"examen", peso, fecha, instrucciones, requiereEntrega, extensiones: "pdf,docx,zip", maxMb, permiteTardia }`

| Método y ruta | Cuerpo / Respuesta |
|---|---|
| `GET /cursos/{id}/evaluaciones` | `[Evaluacion]` ordenadas por fecha (también lo usa el estudiante) |
| `POST /cursos/{id}/evaluaciones` | `Evaluacion` sin id → `Evaluacion`. Valida: nombre, peso 1–100, **suma de pesos del curso ≤ 100**, si `requiereEntrega` entonces extensiones y `maxMb` (1–25) |
| `PUT /evaluaciones/{id}` | igual |
| `DELETE /evaluaciones/{id}` | 204 · borra notas y entregas (y sus archivos) |

## Calificaciones (docente del curso)

| `GET /cursos/{id}/libro` | → `{ evaluaciones: [Evaluacion], filas: [{ estudianteId, nombre, promedio, asistencia, riesgo, notas: { "<evaluacionId>": number|null }, entregas: { "<evaluacionId>": boolean } }] }` |
|---|---|
| `PUT /evaluaciones/{id}/notas/{estudianteId}` | `{ nota }` (0–100 o `null` para borrar) → `{ promedio, riesgo }` |

## Asistencia (docente del curso)

| `GET /cursos/{id}/asistencia?fecha=YYYY-MM-DD` | → `{ fecha, registrada, registros: [{ estudianteId, nombre, presente }] }` (si no hay registro, todos `presente: true`) |
|---|---|
| `PUT /cursos/{id}/asistencia` | `{ fecha, registros: [{ estudianteId, presente }] }` → 204 (reemplaza la lista de esa fecha) |

## Materiales (docente sube, todos del curso leen)

`Material = { id, cursoId, titulo, nombreOriginal, tamano (bytes), fecha (fechaHora) }`

| `GET /cursos/{id}/materiales` | → `[Material]` más recientes primero |
|---|---|
| `POST /cursos/{id}/materiales` | `multipart`: `titulo`, `archivo` (máx. 25 MB) → `Material` |
| `DELETE /materiales/{id}` | 204 |
| `GET /materiales/{id}/archivo` | binario con `Content-Disposition: attachment; filename="..."` |

## Entregas (el estudiante sube, el docente revisa)

`Entrega = { id, evaluacionId, estudianteId, nombreOriginal, tamano, fecha (fechaHora), tardia, comentario }`

| Método y ruta | Quién | Cuerpo / Respuesta |
|---|---|---|
| `GET /evaluaciones/{id}/entregas` | docente | `{ evaluacion, filas: [{ estudianteId, nombre, entrega: Entrega|null, nota }] }` (todos los matriculados) |
| `POST /evaluaciones/{id}/entrega` | estudiante | `multipart`: `archivo`, `comentario` → `Entrega`. **Reemplaza** la anterior (borra su archivo). Rechazar si: la evaluación no pide archivo · ya tiene nota · pasó la `fecha` (fin del día) y `permiteTardia` es falso · extensión fuera de `extensiones` o de la lista global · pesa más de `maxMb` (o 25 MB) · archivo vacío. `tardia` = pasó la fecha y se permitió |
| `DELETE /evaluaciones/{id}/entrega` | estudiante | 204 · no si ya tiene nota |
| `GET /entregas/{id}/archivo` | dueño o docente del curso | binario con `Content-Disposition` |

### Lista global de extensiones permitidas
`pdf doc docx xls xlsx ppt pptx odt txt md csv zip rar 7z png jpg jpeg sql c cs js py java`
(sin ejecutables ni `html`/`svg`). Ver `src/config.js`.

### Almacenamiento de archivos (recomendación)
Guardar **fuera de `wwwroot`** (o en Blob Storage), con nombre aleatorio (GUID), y guardar en la base solo
`nombreOriginal`, `rutaInterna`, `tamano`. Servir siempre por un endpoint que verifique permisos. Comprobar que la
ruta resuelta no escape de la carpeta base.

## Modelo de datos (tablas sugeridas para SQL Server)

| Tabla | Columnas principales |
|---|---|
| `Usuario` | Id, Nombre, Correo (único), ClaveHash, Rol, Activo |
| `Curso` | Id, Codigo (único), Nombre, Periodo, Descripcion, DocenteId → Usuario |
| `Inscripcion` | CursoId → Curso, EstudianteId → Usuario (PK compuesta) |
| `Evaluacion` | Id, CursoId, Nombre, Tipo, Peso, Fecha, Instrucciones, RequiereEntrega, Extensiones, MaxMb, PermiteTardia |
| `Calificacion` | EvaluacionId, EstudianteId (PK compuesta), Nota decimal(5,2) |
| `Asistencia` | CursoId, Fecha, EstudianteId (PK compuesta), Presente bit |
| `Material` | Id, CursoId, Titulo, NombreOriginal, RutaInterna, Tamano, Fecha |
| `Entrega` | Id, EvaluacionId, EstudianteId (único en conjunto), NombreOriginal, RutaInterna, Tamano, Fecha, Tardia, Comentario |

Borrado en cascada: Curso → Evaluacion → (Calificacion, Entrega); Curso → (Inscripcion, Asistencia, Material).
Promedio y riesgo **se calculan** (no se guardan): reglas en `src/utils/calculos.js`.
