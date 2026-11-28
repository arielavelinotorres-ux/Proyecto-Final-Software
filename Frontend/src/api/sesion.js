// El backend actual no emite tokens: solo se conserva el perfil para la interfaz de esta pestaña.
const CLAVE = 'homeclass-usuario'

export function leerUsuario() {
  try {
    const valor = sessionStorage.getItem(CLAVE)
    if (!valor) return null
    const usuario = JSON.parse(valor)
    const roles = ['admin', 'docente', 'estudiante']
    if (
      !usuario ||
      !Number.isInteger(usuario.id) ||
      usuario.id < 1 ||
      typeof usuario.correo !== 'string' ||
      typeof usuario.nombre !== 'string' ||
      !roles.includes(usuario.rol) ||
      usuario.activo !== true
    ) return null
    return usuario
  } catch {
    return null
  }
}
export function guardarUsuario(usuario) {
  try {
    sessionStorage.setItem(CLAVE, JSON.stringify(usuario))
  } catch {
    throw new Error('No se pudo conservar la sesión en esta pestaña. Habilita el almacenamiento del navegador e inténtalo de nuevo.')
  }
}
export function borrarUsuario() {
  try {
    sessionStorage.removeItem(CLAVE)
  } catch {
    throw new Error('No se pudo limpiar la sesión local del navegador.')
  }
}
