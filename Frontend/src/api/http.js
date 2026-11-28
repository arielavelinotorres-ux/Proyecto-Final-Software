// Única implementación de datos: habla con el backend por HTTP/JSON.
// Qué debe devolver cada endpoint está documentado en docs/API.md.
import { API_URL } from '../config.js'

async function pedir(metodo, ruta, { cuerpo } = {}) {
  const cabeceras = {}
  let body
  if (cuerpo !== undefined) {
    cabeceras['Content-Type'] = 'application/json'
    body = JSON.stringify(cuerpo)
  }

  let res
  try {
    res = await fetch(`${API_URL}${ruta}`, { method: metodo, headers: cabeceras, body })
  } catch {
    throw new Error('No se pudo conectar con la API. Comprueba que el backend esté iniciado y que VITE_PROXY apunte a su dirección.')
  }

  if (!res.ok) {
    let mensaje = `Error ${res.status}`
    try {
      const error = await res.json()
      mensaje = error.mensaje ?? error.detail ?? error.title ?? mensaje
    } catch { /* La respuesta puede no incluir JSON. */ }
    throw new Error(mensaje)
  }
  return res.status === 204 ? undefined : res.json()
}

const get = (r) => pedir('GET', r)
const post = (r, cuerpo) => pedir('POST', r, { cuerpo })
const put = (r, cuerpo) => pedir('PUT', r, { cuerpo })
const patch = (r, cuerpo) => pedir('PATCH', r, { cuerpo })
const del = (r) => pedir('DELETE', r)

export const httpApi = {
  auth: {
    login: (correo, password) => post('/usuarios/login', { correo, password }),
  },
  usuarios: {
    listar: () => get('/usuarios'),
    crear: (datos) => post('/usuarios/', datos),
    actualizarPerfil: (id, datos) => put(`/usuarios/${id}/perfil`, datos),
    actualizarPassword: (id, nuevoPassword) => put(`/usuarios/${id}/password`, { nuevoPassword }),
    activar: (id) => patch(`/usuarios/${id}/activar`),
    desactivar: (id) => patch(`/usuarios/${id}/desactivar`),
    cerrarSesiones: (id) => post(`/usuarios/${id}/cerrar-sesiones`, {}),
  },
  docentes: {
    listar: () => get('/docentes/'),
    crear: (datos) => post('/docentes/', datos),
    actualizar: (id, datos) => put(`/docentes/${id}`, datos),
    eliminar: (id) => del(`/docentes/${id}`),
  },
  estudiantes: {
    listar: () => get('/estudiantes/'),
    obtenerPorUsuario: (usuarioId) => get(`/estudiantes/usuario/${usuarioId}`),
    crear: (datos) => post('/estudiantes/', datos),
    actualizar: (id, datos) => put(`/estudiantes/${id}`, datos),
    eliminar: (id) => del(`/estudiantes/${id}`),
  },
  recuperacionPassword: {
    generar: (datos) => post('/recuperacion-password/generar', datos),
    validar: (token) => post('/recuperacion-password/validar', { token }),
    marcarUsado: (id) => patch(`/recuperacion-password/${id}/usar`),
  },
  foros: {
    listarPorCurso: (cursoId) => get(`/foros/curso/${cursoId}`),
    crear: (datos) => post('/foros/', datos),
    actualizar: (id, datos) => put(`/foros/${id}`, datos),
    archivar: (id) => patch(`/foros/${id}/archivar`),
  },
  publicaciones: {
    listarPorForo: (foroId) => get(`/publicaciones-foro/foro/${foroId}`),
    crear: (datos) => post('/publicaciones-foro/', datos),
    actualizar: (id, datos) => put(`/publicaciones-foro/${id}`, datos),
    eliminar: (id) => del(`/publicaciones-foro/${id}`),
  },
  mensajes: {
    recibidos: (usuarioId) => get(`/mensajes-privados/recibidos/${usuarioId}`),
    enviados: (usuarioId) => get(`/mensajes-privados/enviados/${usuarioId}`),
    enviar: (datos) => post('/mensajes-privados/', datos),
    marcarLeido: (id) => patch(`/mensajes-privados/${id}/leer`),
  },
  notificaciones: {
    listarPorUsuario: (usuarioId) => get(`/notificaciones/usuario/${usuarioId}`),
    crear: (datos) => post('/notificaciones/', datos),
    marcarLeida: (id) => patch(`/notificaciones/${id}/leer`),
    marcarTodasLeidas: (usuarioId) => patch(`/notificaciones/usuario/${usuarioId}/leer-todas`),
    eliminar: (id) => del(`/notificaciones/${id}`),
  },
}
