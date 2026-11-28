import { createContext, useCallback, useContext, useMemo, useState } from 'react'
import { api } from '../api/index.js'
import { borrarUsuario, guardarUsuario, leerUsuario } from '../api/sesion.js'

// CONTEXTO: comparte quién inició sesión con TODOS los componentes sin pasar props nivel por nivel.
const AuthContext = createContext(null)

function adaptarUsuario(respuesta) {
  const rolOriginal = respuesta.rolNombre?.normalize('NFD').replace(/\p{Diacritic}/gu, '').trim().toLowerCase()
  const roles = {
    admin: 'admin',
    administrador: 'admin',
    docente: 'docente',
    estudiante: 'estudiante',
  }
  const rol = roles[rolOriginal]
  if (!rol) throw new Error(`El backend devolvió un rol no compatible: ${respuesta.rolNombre ?? 'sin rol'}.`)
  if (!respuesta.id || !respuesta.correo || !respuesta.activo) {
    throw new Error('El backend devolvió datos de cuenta incompletos o inactivos.')
  }

  return {
    id: respuesta.id,
    rol,
    rolNombre: respuesta.rolNombre,
    correo: respuesta.correo,
    nombre: respuesta.correo.split('@')[0],
    activo: respuesta.activo,
  }
}

export function AuthProvider({ children }) {
  const [usuario, setUsuario] = useState(leerUsuario)
  const cargando = false

  const ingresar = useCallback(async (correo, clave) => {
    const respuesta = await api.auth.login(correo, clave)
    const u = adaptarUsuario(respuesta)
    guardarUsuario(u)
    setUsuario(u)
    return u
  }, [])

  const salir = useCallback(() => {
    borrarUsuario()
    setUsuario(null)
  }, [])

  const valor = useMemo(() => ({ usuario, cargando, ingresar, salir, setUsuario }), [usuario, cargando, ingresar, salir])
  return <AuthContext.Provider value={valor}>{children}</AuthContext.Provider>
}

export function useAuth() {
  return useContext(AuthContext)
}
