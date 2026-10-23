// Única implementación de datos: habla con el backend por HTTP (JSON y multipart).
// Qué debe devolver cada endpoint está documentado en docs/API.md.
import { API_URL } from '../config.js'
import { leerToken } from './sesion.js'

async function pedir(metodo, ruta, { cuerpo, formData, blob } = {}) {
  const cabeceras = {}
  const token = leerToken()
  if (token) cabeceras.Authorization = `Bearer ${token}`
  let body
  if (formData) body = formData // el navegador pone el Content-Type multipart solo
  else if (cuerpo !== undefined) {
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
  if (blob) {
    const nombre = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(res.headers.get('Content-Disposition') || '')?.[1]
    return { blob: await res.blob(), nombre: nombre ? decodeURIComponent(nombre) : 'archivo' }
  }
  return res.status === 204 ? undefined : res.json()
}

const get = (r, o) => pedir('GET', r, o)
const post = (r, cuerpo) => pedir('POST', r, { cuerpo })
const put = (r, cuerpo) => pedir('PUT', r, { cuerpo })
const del = (r) => pedir('DELETE', r)

export const httpApi = {
  auth: {
    login: (correo, clave) => post('/auth/login', { correo, clave }),
    solicitarRestablecimiento: (correo) => post('/auth/solicitar-restablecimiento', { correo }),
    sesion: () => get('/auth/sesion'),
    salir: async () => {},
  },
  dashboard: { obtener: () => get('/dashboard') },
  admin: { auditoria: (limite = 100) => get(`/admin/auditoria?limite=${limite}`) },
  usuarios: {
    listar: () => get('/usuarios'),
    crear: (d) => post('/usuarios', d),
    actualizar: (id, d) => put(`/usuarios/${id}`, d),
    restablecerClave: (id, nueva) => post(`/usuarios/${id}/restablecer-clave`, { nueva }),
    eliminar: (id) => del(`/usuarios/${id}`),
  },
  perfil: {
    actualizar: (d) => put('/perfil', d),
    cambiarClave: (d) => put('/perfil/clave', d),
  },
  cursos: {
    listar: () => get('/cursos'),
    obtener: (id) => get(`/cursos/${id}`),
    crear: (d) => post('/cursos', d),
    actualizar: (id, d) => put(`/cursos/${id}`, d),
    eliminar: (id) => del(`/cursos/${id}`),
    matricular: (cursoId, estudianteId) => post(`/cursos/${cursoId}/estudiantes`, { estudianteId }),
    desmatricular: (cursoId, estudianteId) => del(`/cursos/${cursoId}/estudiantes/${estudianteId}`),
    miCurso: (id) => get(`/cursos/${id}/mi-curso`),
  },
  evaluaciones: {
    listar: (cursoId) => get(`/cursos/${cursoId}/evaluaciones`),
    guardar: (cursoId, d) =>
      d.id ? put(`/evaluaciones/${d.id}`, d) : post(`/cursos/${cursoId}/evaluaciones`, d),
    eliminar: (id) => del(`/evaluaciones/${id}`),
  },
  calificaciones: {
    libro: (cursoId) => get(`/cursos/${cursoId}/libro`),
    guardarNota: (evaluacionId, estudianteId, nota) =>
      put(`/evaluaciones/${evaluacionId}/notas/${estudianteId}`, { nota }),
  },
  asistencia: {
    obtener: (cursoId, fecha) => get(`/cursos/${cursoId}/asistencia?fecha=${encodeURIComponent(fecha)}`),
    guardar: (cursoId, fecha, registros) => put(`/cursos/${cursoId}/asistencia`, { fecha, registros }),
  },
  materiales: {
    listar: (cursoId) => get(`/cursos/${cursoId}/materiales`),
    subir: (cursoId, titulo, archivo) => {
      const f = new FormData()
      f.append('titulo', titulo)
      f.append('archivo', archivo)
      return pedir('POST', `/cursos/${cursoId}/materiales`, { formData: f })
    },
    eliminar: (id) => del(`/materiales/${id}`),
    descargar: (id) => get(`/materiales/${id}/archivo`, { blob: true }),
  },
  entregas: {
    listarPorEvaluacion: (evaluacionId) => get(`/evaluaciones/${evaluacionId}/entregas`),
    entregar: (evaluacionId, archivo, comentario) => {
      const f = new FormData()
      f.append('archivo', archivo)
      f.append('comentario', comentario ?? '')
      return pedir('POST', `/evaluaciones/${evaluacionId}/entrega`, { formData: f })
    },
    retirar: (evaluacionId) => del(`/evaluaciones/${evaluacionId}/entrega`),
    descargar: (id) => get(`/entregas/${id}/archivo`, { blob: true }),
  },
  avisos: {
    listar: (cursoId) => get(`/cursos/${cursoId}/avisos`),
    publicar: (cursoId, d) => post(`/cursos/${cursoId}/avisos`, d),
  },
  agenda: { listar: () => get('/agenda') },
  estudiante: { historial: () => get('/estudiante/historial') },
}
