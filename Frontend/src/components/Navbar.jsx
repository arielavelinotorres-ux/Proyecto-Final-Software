import { Link, NavLink, useNavigate } from 'react-router-dom'
import useTheme from '../hooks/useTheme.js'
import { useAuth } from '../context/AuthContext.jsx'

// Barra de la parte pública (inicio e ingreso).
export default function Navbar() {
  const { tema, alternar } = useTheme()
  const { usuario, salir } = useAuth()
  const navegar = useNavigate()
  const secciones = [
    ['nosotros', 'Nosotros'], ['historia', 'Historia'], ['gestiones', 'Gestiones'],
    ['funciones', 'Funciones'], ['contacto', 'Contacto'],
  ]

  async function alSalir() {
    await salir()
    navegar('/')
  }

  return (
    <header className="nav">
      <Link className="brand" to="/" aria-label="HomeClass, inicio">
        <span className="brand-mark"><i className="bi bi-house-heart-fill" /></span>
        <span>
          <strong className="brand-name">HomeClass</strong>
          <small>Entorno Académico</small>
        </span>
      </Link>

      <nav className="nav-secciones" aria-label="Secciones de HomeClass">
        {secciones.map(([ruta, nombre]) => <NavLink key={ruta} to={`/${ruta}`} end>{nombre}</NavLink>)}
      </nav>

      <div className="nav-actions">
        <button type="button" className="icon-btn" onClick={alternar} aria-label={tema === 'dark' ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro'}>
          <i className={tema === 'dark' ? 'bi bi-sun' : 'bi bi-moon-stars'} />
        </button>
        {usuario ? (
          <>
            <Link className="btn btn-tiza" to="/app">Mi panel</Link>
            <button type="button" className="btn btn-claro" onClick={alSalir}>Salir</button>
          </>
        ) : (
          <Link className="btn btn-tiza" to="/ingresar">Ingresar</Link>
        )}
      </div>
    </header>
  )
}
