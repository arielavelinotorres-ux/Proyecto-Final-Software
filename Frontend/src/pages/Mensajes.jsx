import { useState } from 'react'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import { useAviso } from '../context/ToastContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { Cargando, Encabezado, ErrorCaja, Vacio } from '../components/ui.jsx'

export default function Mensajes() {
  const { usuario } = useAuth()
  const avisar = useAviso()
  const { datos: mensajes, cargando, error, recargar } = useAsync(() => api.mensajes.recibidos(usuario.id), [usuario.id])
  const { datos: usuarios, cargando: cargandoContactos, error: errorContactos, recargar: recargarContactos } = useAsync(() => api.usuarios.listar(), [])
  const [form, setForm] = useState({ receptorId: '', asunto: '', contenido: '' })
  const [errorEnvio, setErrorEnvio] = useState(null)
  const [enviando, setEnviando] = useState(false)
  const contactos = (usuarios ?? []).filter((item) => item.id !== usuario.id && item.activo)

  async function enviar(event) {
    event.preventDefault()
    setEnviando(true)
    setErrorEnvio(null)
    try {
      await api.mensajes.enviar({
        emisorId: usuario.id,
        receptorId: Number(form.receptorId),
        asunto: form.asunto.trim() || null,
        contenido: form.contenido.trim(),
      })
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
      <Encabezado titulo="Mensajes privados" subtitulo="Bandeja y envíos conectados con la API de comunicación." />
      <section className="tarjeta">
        <h2>Enviar mensaje</h2>
        <form className="form" onSubmit={enviar}>
          <div className="campo">
            <label htmlFor="mensaje-receptor">Destinatario</label>
            <select id="mensaje-receptor" value={form.receptorId} onChange={(event) => setForm({ ...form, receptorId: event.target.value })} disabled={cargandoContactos || Boolean(errorContactos)} required>
              <option value="">Selecciona un usuario</option>
              {contactos.map((item) => (
                <option key={item.id} value={item.id}>{item.correo} · {item.rolNombre}</option>
              ))}
            </select>
          </div>
          {errorContactos && <ErrorCaja mensaje={errorContactos} alReintentar={recargarContactos} />}
          <div className="campo"><label htmlFor="mensaje-asunto">Asunto</label><input id="mensaje-asunto" maxLength="150" value={form.asunto} onChange={(event) => setForm({ ...form, asunto: event.target.value })} /></div>
          <div className="campo"><label htmlFor="mensaje-contenido">Mensaje (máximo 200 caracteres)</label><textarea id="mensaje-contenido" maxLength="200" value={form.contenido} onChange={(event) => setForm({ ...form, contenido: event.target.value })} required /></div>
          {errorEnvio && <ErrorCaja mensaje={errorEnvio} />}
          <button type="submit" className="btn btn-primario" disabled={enviando || !form.receptorId || contactos.length === 0}>{enviando ? 'Enviando…' : 'Enviar'}</button>
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
              <small className="suave">De {mensaje.emisorNombre} · {new Date(mensaje.fechaEnvio).toLocaleString()}</small>
            </div>
            {!mensaje.leido && <button className="btn btn-claro btn-chico" type="button" onClick={() => marcarLeido(mensaje.id)}>Marcar leído</button>}
          </li>
        ))}</ul>
      </section>
    </>
  )
}
