import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import { api } from '../api/index.js'

export default function Ingresar() {
  const { usuario, ingresar } = useAuth()
  const navegar = useNavigate()
  const { state } = useLocation()
  const [correo, setCorreo] = useState('')
  const [clave, setClave] = useState('')
  const [error, setError] = useState(null)
  const [mensaje, setMensaje] = useState(null)
  const [enviando, setEnviando] = useState(false)
  const [modoRecuperacion, setModoRecuperacion] = useState(false)

  if (usuario) return <Navigate to={state?.desde ?? '/app'} replace />

  async function alEnviar(e) {
    e.preventDefault()
    setError(null)
    setMensaje(null)
    setEnviando(true)
    try {
      if (modoRecuperacion) {
        const respuesta = await api.auth.solicitarRestablecimiento(correo)
        setMensaje(respuesta.mensaje)
      } else {
        await ingresar(correo, clave)
        navegar(state?.desde ?? '/app', { replace: true })
      }
    } catch (err) {
      setError(err.message)
    } finally {
      setEnviando(false)
    }
  }

  return (
    <section className="login">
      <h1>Ingresar a HomeClass</h1>
      <p className="suave">{modoRecuperacion ? 'Solicita al administrador restablecer tu contraseña.' : 'Entra con tu cuenta para ver tu panel.'}</p>

      <form className="form" onSubmit={alEnviar} noValidate={false}>
        <div className="campo">
          <label htmlFor="correo">Correo</label>
          <input id="correo" type="email" value={correo} onChange={(e) => setCorreo(e.target.value)} required autoComplete="username" />
        </div>
        {!modoRecuperacion && <div className="campo">
          <label htmlFor="clave">Contraseña</label>
          <input id="clave" type="password" value={clave} onChange={(e) => setClave(e.target.value)} required autoComplete="current-password" />
        </div>}
        <div aria-live="polite">
          {error && <p className="msg msg-error">{error}</p>}
          {mensaje && <p className="msg msg-ok">{mensaje}</p>}
        </div>
        <button type="submit" className="btn btn-primario" disabled={enviando}>{enviando ? 'Procesando…' : modoRecuperacion ? 'Solicitar restablecimiento' : 'Ingresar'}</button>
        <button type="button" className="enlace" onClick={() => { setModoRecuperacion(!modoRecuperacion); setError(null); setMensaje(null) }}>
          {modoRecuperacion ? 'Volver al inicio de sesión' : 'Olvidé mi contraseña'}
        </button>
      </form>

    </section>
  )
}
