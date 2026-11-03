import { useState } from 'react'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import { useAviso } from '../context/ToastContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { Cargando, Encabezado, ErrorCaja, Vacio } from '../components/ui.jsx'

export default function Mensajes() {
  const { usuario } = useAuth()
  const avisar = useAviso()
  const { datos: mensajes, cargando, error, recargar } = useAsync(() => api.mensajes.listar(), [])
  const { datos: usuarios, error: errorContactos, recargar: recargarContactos } = useAsync(() => api.mensajes.contactos(), [])
  const [form, setForm] = useState({ receptorId: '', asunto: '', contenido: '' })
  const [errorEnvio, setErrorEnvio] = useState(null)
  const [enviando, setEnviando] = useState(false)

  async function enviar(event) {
    event.preventDefault()
    setEnviando(true)
    setErrorEnvio(null)
    try {
      await api.mensajes.enviar({ ...form, receptorId: Number(form.receptorId) })
      setForm({ receptorId: '', asunto: '', contenido: '' })
      avisar('Mensaje enviado.')
      recargar()
    } catch (err) {
      setErrorEnvio(err.message)
    } finally {
      setEnviando(false)
    }
  }

  async function marcarLeido(id) {
    try {
      await api.mensajes.marcarLeido(id)
      recargar()
    } catch (err) {
      setErrorEnvio(err.message)
    }
  }

  return (
    <>
      <Encabezado titulo="Mensajes privados" subtitulo="Bandeja y envíos guardados en Comunicacion.mensajes_privados." />
      <section className="tarjeta">
        <h2>Enviar mensaje</h2>
        <form className="form" onSubmit={enviar}>
          <div className="campo">
            <label htmlFor="mensaje-receptor">Destinatario</label>
            <select id="mensaje-receptor" value={form.receptorId} onChange={(event) => setForm({ ...form, receptorId: event.target.value })} required>
              <option value="">Selecciona un usuario</option>
              {usuarios?.filter((item) => item.id !== usuario.id).map((item) => (
                <option key={item.id} value={item.id}>{item.nombre} · {item.rol}</option>
              ))}
            </select>
          </div>
          {errorContactos && <ErrorCaja mensaje={errorContactos} alReintentar={recargarContactos} />}
          <div className="campo"><label htmlFor="mensaje-asunto">Asunto</label><input id="mensaje-asunto" maxLength="150" value={form.asunto} onChange={(event) => setForm({ ...form, asunto: event.target.value })} /></div>
          <div className="campo"><label htmlFor="mensaje-contenido">Mensaje (máximo 200 caracteres)</label><textarea id="mensaje-contenido" maxLength="200" value={form.contenido} onChange={(event) => setForm({ ...form, contenido: event.target.value })} required /></div>
          {errorEnvio && <ErrorCaja mensaje={errorEnvio} />}
          <button type="submit" className="btn btn-primario" disabled={enviando}>{enviando ? 'Enviando…' : 'Enviar'}</button>
        </form>
      </section>
      <section className="tarjeta">
        <h2>Recibidos</h2>
        {cargando && <Cargando />}
        {error && <ErrorCaja mensaje={error} alReintentar={recargar} />}
        {mensajes?.length === 0 && <Vacio titulo="Sin mensajes" texto="Los mensajes recibidos aparecerán aquí." />}
        <ul className="lista">{mensajes?.map((mensaje) => (
          <li key={mensaje.id}>
            <div>
              <strong>{mensaje.asunto || 'Sin asunto'}</strong>
              <p>{mensaje.contenido}</p>
              <small className="suave">De {mensaje.remitente} · {new Date(mensaje.fecha_envio).toLocaleString()}</small>
            </div>
            {!mensaje.leido && <button className="btn btn-claro btn-chico" type="button" onClick={() => marcarLeido(mensaje.id)}>Marcar leído</button>}
          </li>
        ))}</ul>
      </section>
    </>
  )
}
