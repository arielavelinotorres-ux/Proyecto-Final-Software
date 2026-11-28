import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

export default function PanelLms() {
  const { usuario } = useAuth()

  return (
    <>
      <section className="panel-bienvenida">
        <div>
          <p className="panel-kicker">HOMECLASS</p>
          <h2>¡Hola, {usuario.nombre}!</h2>
          <p>Conectado como {usuario.correo} · {usuario.rolNombre}</p>
        </div>
        <Link className="btn btn-primario" to="/app/mensajes">
          <i className="bi bi-chat-dots" /> Abrir mensajes
        </Link>
      </section>

      <section className="tarjeta">
        <h2>Funciones disponibles</h2>
        <p>Esta versión del backend expone autenticación básica de credenciales, administración de usuarios y comunicación. La bandeja privada ya está conectada a sus endpoints de recibidos, envío y lectura.</p>
        <p className="msg msg-info">La API actual no publica endpoints de cursos, catálogo, contenido académico ni dashboard. Esas pantallas no se muestran para evitar datos simulados y solicitudes a rutas inexistentes.</p>
        <p className="suave">Nota: el backend no emite un token ni protege sus endpoints con autorización. El perfil guardado aquí solo controla la interfaz; no constituye una sesión segura.</p>
      </section>
    </>
  )
}
