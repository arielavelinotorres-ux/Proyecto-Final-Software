import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { api } from '../api/index.js'
import { borrarToken, guardarToken, leerToken } from '../api/sesion.js'

// CONTEXTO: comparte quién inició sesión con TODOS los componentes sin pasar props nivel por nivel.
const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [cargando, setCargando] = useState(Boolean(leerToken())) // ¿hay sesión guardada por verificar?

  // Al abrir la página, si había una sesión guardada, pedimos los datos del usuario al "servidor".
  useEffect(() => {
    if (!leerToken()) return
    let vigente = true
    api.auth.sesion()
      .then((u) => vigente && setUsuario(u))
      .catch(() => borrarToken())
      .finally(() => vigente && setCargando(false))
    return () => { vigente = false }
  }, [])

  const ingresar = useCallback(async (correo, clave) => {
    const { token, usuario: u } = await api.auth.login(correo, clave)
    guardarToken(token)
    setUsuario(u)
    return u
  }, [])

  const salir = useCallback(async () => {
    try {
      await api.auth.salir()
    } finally {
      borrarToken()
      setUsuario(null)
    }
  }, [])

  const valor = useMemo(() => ({ usuario, cargando, ingresar, salir, setUsuario }), [usuario, cargando, ingresar, salir])
  return <AuthContext.Provider value={valor}>{children}</AuthContext.Provider>
}

export function useAuth() {
  return useContext(AuthContext)
}
