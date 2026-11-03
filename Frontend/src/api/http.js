// Única implementación de datos: habla con el backend por HTTP/JSON.
// Qué debe devolver cada endpoint está documentado en docs/API.md.
import { API_URL } from '../config.js'
import { leerToken } from './sesion.js'

async function pedir(metodo, ruta, { cuerpo } = {}) {
  const cabeceras = {}
  const token = leerToken()
  if (token) cabeceras.Authorization = `Bearer ${token}`
  let body
  if (cuerpo !== undefined) {
    cabeceras['Content-Type'] = 'application/json'
    body = JSON.stringify(cuerpo)
  }

  let res
  try {
    res = await fetch(`${API_URL}${ruta}`, { method: metodo, headers: cabeceras, body })
  } catch {
    throw new Error('No se pudo conectar con el servidor. Revisa tu conexión.')
  }

  if (!res.ok) {
    const tipoContenido = res.headers.get('Content-Type') || ''
    if (res.status === 404 && !tipoContenido.includes('application/json')) {
      throw new Error('El backend todavía no está disponible. Configura VITE_PROXY cuando esté construido.')
    }
    // El backend debe responder { "mensaje": "..." } en los errores.
    let mensaje = `Error ${res.status}`
    try { mensaje = (await res.json()).mensaje ?? mensaje } catch { /* sin cuerpo */ }
    if (res.status === 401) mensaje = mensaje === `Error 401` ? 'Tu sesión expiró. Ingresa de nuevo.' : mensaje
    throw new Error(mensaje)
  }
  return res.status === 204 ? undefined : res.json()
}

const get = (r) => pedir('GET', r)
const post = (r, cuerpo) => pedir('POST', r, { cuerpo })
const put = (r, cuerpo) => pedir('PUT', r, { cuerpo })
const del = (r) => pedir('DELETE', r)

export const httpApi = {
  auth: {
    login: (correo, clave) => post('/auth/login', { correo, clave }),
    sesion: () => get('/auth/sesion'),
    salir: () => post('/auth/salir', {}),
  },
  dashboard: { obtener: () => get('/dashboard') },
  catalogo: {
    categorias: () => get('/categorias'),
    crearCategoria: (d) => post('/categorias', d),
    actualizarCategoria: (id, d) => put(`/categorias/${id}`, d),
    eliminarCategoria: (id) => del(`/categorias/${id}`),
    subcategorias: () => get('/subcategorias'),
    crearSubcategoria: (d) => post('/subcategorias', d),
    actualizarSubcategoria: (id, d) => put(`/subcategorias/${id}`, d),
    eliminarSubcategoria: (id) => del(`/subcategorias/${id}`),
  },
  usuarios: { listar: (rol) => get(rol ? `/usuarios?rol=${encodeURIComponent(rol)}` : '/usuarios') },
  cursos: {
    listar: () => get('/cursos'),
    obtener: (id) => get(`/cursos/${id}`),
    crear: (d) => post('/cursos', d),
    actualizar: (id, d) => put(`/cursos/${id}`, d),
    eliminar: (id) => del(`/cursos/${id}`),
    matricular: (cursoId, estudianteId) => post(`/cursos/${cursoId}/estudiantes`, { estudianteId }),
    desmatricular: (cursoId, estudianteId) => del(`/cursos/${cursoId}/estudiantes/${estudianteId}`),
    actualizarMatricula: (cursoId, estudianteId, estado) => put(`/cursos/${cursoId}/estudiantes/${estudianteId}`, { estado }),
    contenido: (id) => get(`/cursos/${id}/contenido`),
    actualizarProgreso: (leccionId, completado) => put(`/lecciones/${leccionId}/progreso`, { completado }),
    foros: (id) => get(`/cursos/${id}/foros`),
    crearForo: (id, d) => post(`/cursos/${id}/foros`, d),
    actualizarForo: (id, d) => put(`/foros/${id}`, d),
    archivarForo: (id) => del(`/foros/${id}`),
  },
  foros: {
    publicaciones: (id) => get(`/foros/${id}/publicaciones`),
    publicar: (id, d) => post(`/foros/${id}/publicaciones`, d),
  },
  mensajes: {
    listar: () => get('/mensajes'),
    contactos: () => get('/contactos'),
    enviar: (d) => post('/mensajes', d),
    marcarLeido: (id) => put(`/mensajes/${id}/leido`, {}),
  },
}
