export function tienePermisoAdmin(usuario, permiso) {
  if (usuario?.rol !== 'admin') return false
  const permisos = usuario.permisosAdmin ?? []
  if (permisos.includes('general') || permisos.includes(permiso)) return true
  const separador = permiso.indexOf('.')
  if (separador > 0) return permisos.includes(permiso.slice(0, separador))
  return permisos.some((concedido) => concedido.startsWith(`${permiso}.`))
}