# Informe técnico: integración del frontend con el backend

- **Proyecto:** HomeClass
- **Fecha:** 27 de noviembre de 2026
- **Alcance:** adaptación del frontend; backend sin modificaciones.

## 1. Resumen ejecutivo

Se revisaron los contratos de la Minimal API activa y se adecuó el frontend para consumir las rutas que realmente publica. El cliente HTTP ahora contempla autenticación por credenciales, usuarios y perfiles, recuperación de contraseña, mensajería, foros, publicaciones y notificaciones. La pantalla de mensajes quedó conectada a los DTO y rutas de comunicación del backend.

La API activa no ofrece endpoints para cursos, dashboard, categorías ni contenido LMS. Por ello, esas pantallas no se presentan como funciones operativas en la navegación actual. No se simularon respuestas ni se modificó el backend para completar esas capacidades.

## 2. Alcance y criterios

- Los cambios se limitaron a `Frontend/`.
- Los métodos, rutas, cuerpos y respuestas se contrastaron con los endpoints y DTO actuales del backend.
- Se conservaron únicamente flujos respaldados por rutas reales; las capacidades no disponibles se dejaron fuera de la navegación.
- La integración no agrega autenticación o autorización en el frontend que el backend no proporcione.

## 3. Hallazgos sobre el contrato del backend

La implementación actual difiere del contrato que esperaba el cliente previo:

1. El login está en `POST /api/usuarios/login` y recibe `{ correo, password }`.
2. La respuesta del login contiene `{ id, rolId, rolNombre, correo, activo }`; no incluye `{ token, usuario }`.
3. Las rutas de mensajería están bajo `/api/mensajes-privados`, no bajo `/api/mensajes`.
4. El backend publica rutas de usuarios, perfiles docente/estudiante, recuperación, foros, publicaciones y notificaciones.
5. No publica rutas de cursos, catálogo, dashboard, secciones, lecciones o progreso académico.

Los contratos detallados se mantienen en [`API.md`](./API.md).

## 4. Implementación realizada

### 4.1 Cliente HTTP

[`src/api/http.js`](../src/api/http.js) centraliza las solicitudes `fetch` y expone los métodos disponibles por dominio. Serializa cuerpos JSON, procesa respuestas `204` y muestra mensajes de error que pueden llegar como `mensaje`, `detail` o `title`. Se retiró el encabezado Bearer: el backend actual no emite ni consume ese token.

### 4.2 Login y estado de usuario

[`src/context/AuthContext.jsx`](../src/context/AuthContext.jsx) envía las credenciales con los nombres esperados por el DTO del backend y adapta los roles `Administrador`, `Docente` y `Estudiante` a los identificadores internos del frontend.

Como el login no devuelve una credencial de sesión, [`src/api/sesion.js`](../src/api/sesion.js) conserva el perfil en `sessionStorage` solo para mantener el estado de interfaz de esa pestaña. Este perfil no valida solicitudes posteriores ni equivale a una sesión autenticada.

### 4.3 Mensajería

[`src/pages/Mensajes.jsx`](../src/pages/Mensajes.jsx) utiliza los contratos reales:

| Acción | Método y ruta |
|---|---|
| Cargar destinatarios | `GET /api/usuarios/` |
| Cargar mensajes recibidos | `GET /api/mensajes-privados/recibidos/{receptorId}` |
| Enviar mensaje | `POST /api/mensajes-privados/` |
| Marcar como leído | `PATCH /api/mensajes-privados/{id}/leer` |

El envío incluye `emisorId`, `receptorId`, `asunto` y `contenido`. La vista consume los campos `emisorNombre` y `fechaEnvio` del DTO de mensajes recibidos.

### 4.4 Navegación y capacidades disponibles

[`src/App.jsx`](../src/App.jsx), [`src/layouts/AppLayout.jsx`](../src/layouts/AppLayout.jsx) y [`src/pages/PanelLms.jsx`](../src/pages/PanelLms.jsx) limitan el panel a las funciones compatibles con las rutas actuales. Cursos y categorías ya no aparecen como opciones operativas. Se añadió estilo para el aviso informativo en [`src/styles.css`](../src/styles.css).

### 4.5 Configuración de conexión

[`vite.config.js`](../vite.config.js) reenvía `/api` al valor de `VITE_PROXY`, cuyo valor predeterminado es `http://localhost:5296`, según el perfil HTTP de `launchSettings.json`. [`.env.example`](../.env.example) contiene la configuración de referencia; debe copiarse a `.env` para personalizar el entorno local. `VITE_API_URL` permanece en `/api`.

## 5. Matriz de compatibilidad

| Área | Estado en el frontend | Observaciones |
|---|---|---|
| Login | Conectado | Se valida la respuesta del backend y se adapta el rol. No hay token ni sesión de servidor. |
| Mensajes | Conectados | Bandeja recibida, envío y marcado como leído. |
| Listado de usuarios | Usado como fuente de destinatarios | No se expone una pantalla de administración de usuarios. |
| Perfiles, recuperación, foros, publicaciones y notificaciones | Métodos disponibles en el cliente HTTP | No se presentan como flujos de interfaz activos. |
| Cursos, dashboard, categorías y contenido LMS | No disponibles | La API activa no publica las rutas necesarias. |

## 6. Seguridad y despliegue

### 6.1 Autorización

El backend actual no devuelve token o cookie durante el login y no se identificó middleware ni policies de autorización para proteger estos endpoints. Las rutas protegidas del frontend solo controlan la navegación de la interfaz. Además, las solicitudes de comunicación reciben identificadores como `emisorId` desde el cliente.

Por lo tanto, un usuario puede modificar el perfil almacenado o construir solicitudes con otros identificadores. Esta limitación no se puede resolver de forma segura solo desde el frontend. **No se debe considerar protegido el acceso ni desplegar este flujo con datos reales hasta implementar autenticación y autorización del lado del servidor.**

### 6.2 CORS y origen de la API

El backend actual no configura CORS. El proxy de Vite aplica al entorno de desarrollo; para producción, frontend y API deben servirse bajo el mismo origen manteniendo `/api`. Configurar una URL cross-origin únicamente en el frontend no habilita la comunicación.

## 7. Archivos modificados

- `Frontend/.env.example`
- `Frontend/docs/API.md`
- `Frontend/src/App.jsx`
- `Frontend/src/api/http.js`
- `Frontend/src/api/sesion.js`
- `Frontend/src/context/AuthContext.jsx`
- `Frontend/src/layouts/AppLayout.jsx`
- `Frontend/src/pages/Mensajes.jsx`
- `Frontend/src/pages/PanelLms.jsx`
- `Frontend/src/styles.css`
- `Frontend/vite.config.js`

## 8. Validación y límites de prueba

- `npm run build` ejecutado desde `Frontend/`: **correcto**.
- `git diff --check`: **correcto**.
- Las rutas y DTO modificados se contrastaron con los endpoints activos del backend.
- No se validó una transacción contra la base de datos ni se utilizaron credenciales reales.
- El backend no respondió en `localhost:5296` durante la revisión; por ello, no se afirma una prueba de integración en vivo.
- No se modificó ni se compiló el backend.

## 9. Ejecución local

Desde la raíz del repositorio, iniciar la API en una terminal:

```powershell
Set-Location Backend\Proyecto\SistemaEducativoDeRiesgo
dotnet run --launch-profile http
```

En otra terminal, iniciar el frontend:

```powershell
Set-Location Frontend
npm run dev
```

Si la API utiliza otro puerto, establecer `VITE_PROXY` en el archivo `.env` del frontend y reiniciar Vite.
