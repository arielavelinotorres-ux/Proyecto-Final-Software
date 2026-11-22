import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import IlustracionEstudio from '../components/IlustracionEstudio.jsx'

export default function Ingresar() {
  const { usuario, ingresar } = useAuth()
  const navegar = useNavigate()
  const { state } = useLocation()
  const [correo, setCorreo] = useState('')
  const [clave, setClave] = useState('')
  const [mostrarClave, setMostrarClave] = useState(false)
  const [error, setError] = useState(null)
  const [enviando, setEnviando] = useState(false)

  if (usuario) return <Navigate to={state?.desde ?? '/app'} replace />

  async function alEnviar(e) {
    e.preventDefault()
    setError(null)
    setEnviando(true)
    try {
      await ingresar(correo, clave)
      navegar(state?.desde ?? '/app', { replace: true })
    } catch (err) {
      setError(err.message)
    } finally {
      setEnviando(false)
    }
  }

  return (
    <section className="login">
      <div className="login-arte">
        <IlustracionEstudio />
        <p className="login-lema">Un espacio para aprender, crear y compartir.</p>
        <span className="login-etiqueta"><i className="bi bi-book-half" aria-hidden="true" /> Cursos, ideas y nuevas metas</span>
      </div>

      <div className="login-panel">
        <div className="login-marca">
          <span className="login-marca-icono"><i className="bi bi-journal-bookmark-fill" aria-hidden="true" /></span>
          <strong>HomeClass</strong>
        </div>
        <h1>¡Qué bueno verte!</h1>
        <p className="login-descripcion">Ingresa a tu cuenta para continuar aprendiendo.</p>

        <form className="form login-form" onSubmit={alEnviar}>
          <div className="campo login-campo">
            <label htmlFor="correo">Correo electrónico</label>
            <input id="correo" type="email" value={correo} onChange={(e) => setCorreo(e.target.value)} required autoComplete="username" />
          </div>
          <div className="campo login-campo">
            <label htmlFor="clave">Contraseña</label>
            <div className="login-clave">
              <input id="clave" type={mostrarClave ? 'text' : 'password'} value={clave} onChange={(e) => setClave(e.target.value)} required autoComplete="current-password" />
              <button
                type="button"
                className="login-ver-clave"
                onClick={() => setMostrarClave((visible) => !visible)}
                aria-label={mostrarClave ? 'Ocultar contraseña' : 'Mostrar contraseña'}
              >
                <i className={`bi ${mostrarClave ? 'bi-eye-slash' : 'bi-eye'}`} aria-hidden="true" />
              </button>
            </div>
          </div>
          {error && <p className="msg msg-error login-error" role="alert">{error}</p>}
          <button type="submit" className="btn login-submit" disabled={enviando}>
            {enviando ? 'Ingresando…' : 'Ingresar a HomeClass'}
            {!enviando && <i className="bi bi-arrow-right" aria-hidden="true" />}
          </button>
        </form>
        <p className="login-ayuda">Usa el correo y la contraseña de una cuenta activa. Las opciones disponibles dependen de tu rol.</p>
      </div>
    </section>
  )
}
