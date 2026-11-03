import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import { useAviso } from '../context/ToastContext.jsx'
import useTheme from '../hooks/useTheme.js'

export default function AppLayout() {
  const { usuario, salir } = useAuth()
  const avisar = useAviso()
  const { tema, alternar } = useTheme()
  const navegar = useNavigate()

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
          <span className="brand-mark"><i className="bi bi-house-heart-fill" /></span>
          <span><strong className="brand-name">HomeClass</strong><small>{usuario.rol}</small></span>
        </NavLink>
        <nav aria-label="Principal" className="menu">
          <NavLink to="/app" end className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
            <i className="bi bi-speedometer2" /><span>Resumen</span>
          </NavLink>
          <NavLink to="/app/cursos" className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
            <i className="bi bi-journal-bookmark" /><span>{usuario.rol === 'admin' ? 'Cursos' : 'Mis cursos'}</span>
          </NavLink>
          {usuario.rol === 'admin' && (
            <NavLink to="/app/categorias" className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
              <i className="bi bi-collection" /><span>Categorías</span>
            </NavLink>
          )}
          <NavLink to="/app/mensajes" className={({ isActive }) => isActive ? 'menu-item activo' : 'menu-item'}>
            <i className="bi bi-envelope" /><span>Mensajes</span>
          </NavLink>
        </nav>
        <div className="lateral-pie">
          <button type="button" className="menu-item" onClick={alternar}>
            <i className={tema === 'dark' ? 'bi bi-sun' : 'bi bi-moon-stars'} />
            <span>{tema === 'dark' ? 'Tema claro' : 'Tema oscuro'}</span>
          </button>
          <button type="button" className="menu-item" onClick={alSalir}>
            <i className="bi bi-box-arrow-left" /><span>Salir</span>
          </button>
          <p className="usuario-mini" title={usuario.correo}>{usuario.nombre}</p>
        </div>
      </aside>
      <main id="contenido" className="contenido" tabIndex={-1}>
        <Outlet />
      </main>
    </div>
  )
}
