import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import { useAviso } from '../context/ToastContext.jsx'
import useTheme from '../hooks/useTheme.js'
import IconoHomeClass from '../components/IconoHomeClass.jsx'

export default function AppLayout() {
  const { usuario, salir } = useAuth()
  const avisar = useAviso()
  const { tema, alternar } = useTheme()
  const navegar = useNavigate()
  const ubicacion = useLocation()
  const pagina = ubicacion.pathname.startsWith('/app/categorias')
    ? 'Categorías'
    : ubicacion.pathname.startsWith('/app/mensajes')
      ? 'Mensajes'
      : ubicacion.pathname.startsWith('/app/cursos/')
        ? 'Curso'
        : ubicacion.pathname.startsWith('/app/cursos')
          ? 'Cursos'
          : 'Resumen'

  async function alSalir() {
    try {
      await salir()
    } catch (error) {
      avisar(error.message, 'error')
    }
    navegar('/')
  }

  return (
    <div className="app" data-rol={usuario.rol}>
      <a className="saltar" href="#contenido">Saltar al contenido</a>
      <aside className="lateral">
        <NavLink to="/" className="brand brand-lateral" aria-label="HomeClass, ir al inicio">
          <span className="brand-mark"><IconoHomeClass tipo="marca" /></span>
          <span><strong className="brand-name">HomeClass</strong><small>Entorno académico</small></span>
        </NavLink>
        <p className="menu-etiqueta">MENÚ PRINCIPAL</p>
        <nav aria-label="Principal" className="menu">
          <NavLink to="/app" end className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
            <IconoHomeClass tipo="resumen" /><span>Resumen</span>
          </NavLink>
          <NavLink to="/app/cursos" className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
            <IconoHomeClass tipo="cursos" /><span>{usuario.rol === 'admin' ? 'Cursos' : 'Mis cursos'}</span>
          </NavLink>
          {usuario.rol === 'admin' && (
            <NavLink to="/app/categorias" className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
              <IconoHomeClass tipo="categorias" /><span>Categorías</span>
            </NavLink>
          )}
          <NavLink to="/app/mensajes" className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
            <IconoHomeClass tipo="comunicacion" /><span>Mensajes</span>
          </NavLink>
        </nav>
        <div className="lateral-pie">
          <div className="usuario-mini" title={usuario.correo}>
            <span className="usuario-avatar" aria-hidden="true">{usuario.nombre?.trim()?.charAt(0)?.toUpperCase() || 'U'}</span>
            <span className="usuario-datos"><strong>{usuario.nombre}</strong><small>{usuario.rol}</small></span>
          </div>
        </div>
      </aside>
      <div className="app-principal">
        <header className="app-bar">
          <div>
            <p className="app-bar-kicker">HomeClass / {usuario.rol}</p>
            <h1>{pagina}</h1>
          </div>
          <div className="app-bar-acciones">
            <button type="button" className="icon-btn" onClick={alternar} aria-label={tema === 'dark' ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro'} title={tema === 'dark' ? 'Tema claro' : 'Tema oscuro'}>
              <i className={tema === 'dark' ? 'bi bi-sun' : 'bi bi-moon-stars'} />
            </button>
            <button type="button" className="btn btn-claro btn-salir" onClick={alSalir}>
              <i className="bi bi-box-arrow-left" /><span>Salir</span>
            </button>
          </div>
        </header>
        <main id="contenido" className="contenido" tabIndex={-1}>
          <Outlet />
        </main>
      </div>
    </div>
  )
}
