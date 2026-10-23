import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import { Cargando } from './ui.jsx'
import { tienePermisoAdmin } from '../utils/permisos.js'

// Protege un grupo de rutas. Si se indica `roles`, solo esos roles pueden entrar.
export default function RutaProtegida({ roles, permisosAdmin }) {
  const { usuario, cargando } = useAuth()
  const ubicacion = useLocation()

  if (cargando) return <Cargando texto="Verificando sesión…" />
  if (!usuario) return <Navigate to="/ingresar" replace state={{ desde: ubicacion.pathname }} />
  if (roles && !roles.includes(usuario.rol)) return <Navigate to="/app" replace />
  if (usuario.rol === 'admin' && permisosAdmin && !permisosAdmin.every((permiso) => tienePermisoAdmin(usuario, permiso))) return <Navigate to="/app" replace />
  return <Outlet />
}
