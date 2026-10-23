import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import useTheme from '../hooks/useTheme.js'
import { tienePermisoAdmin } from '../utils/permisos.js'

// Menú lateral distinto por rol: cada persona ve solo lo que le corresponde.
const MENU = {
  admin: [
    { a: '/app', t: 'Resumen', i: 'bi-speedometer2', fin: true },
    { a: '/app/usuarios', t: 'Usuarios', i: 'bi-people' },
    { a: '/app/seguridad', t: 'Seguridad de cuentas', i: 'bi-shield-lock' },
    { a: '/app/cursos', t: 'Cursos', i: 'bi-journal-bookmark' },
    { a: '/app/reportes', t: 'Reportes', i: 'bi-clipboard-data' },
    { a: '/app/auditoria', t: 'Auditoría', i: 'bi-shield-check' },
    { a: '/app/perfil', t: 'Mi perfil', i: 'bi-person-gear' },
  ],
  docente: [
    { a: '/app', t: 'Resumen', i: 'bi-speedometer2', fin: true },
    { a: '/app/herramientas', t: 'Herramientas docentes', i: 'bi-tools' },
    { a: '/app/cursos', t: 'Mis cursos', i: 'bi-easel' },
    { a: '/app/agenda', t: 'Agenda', i: 'bi-calendar3' },
    { a: '/app/perfil', t: 'Mi perfil', i: 'bi-person-gear' },
  ],
  estudiante: [
    { a: '/app', t: 'Resumen', i: 'bi-speedometer2', fin: true },
    { a: '/app/herramientas', t: 'Actividad académica', i: 'bi-lightning-charge' },
    { a: '/app/cursos', t: 'Mis cursos', i: 'bi-mortarboard' },
    { a: '/app/agenda', t: 'Agenda', i: 'bi-calendar3' },
    { a: '/app/historial', t: 'Historial académico', i: 'bi-clock-history' },
    { a: '/app/perfil', t: 'Mi perfil', i: 'bi-person-gear' },
  ],
}
const ROL = { admin: 'Administración', docente: 'Docente', estudiante: 'Estudiante' }
const TIPO_ADMIN = { general: 'Admin general', personalizado: 'Admin delegado', usuarios: 'Admin de usuarios', cursos: 'Admin de cursos', reportes: 'Admin de reportes' }

export default function AppLayout() {
  const { usuario, salir } = useAuth()
  const { tema, alternar } = useTheme()
  const navegar = useNavigate()
  const menu = usuario.rol === 'admin'
    ? MENU.admin.filter((item) => {
      if (item.a === '/app') return tienePermisoAdmin(usuario, 'general')
      if (item.a === '/app/perfil') return true
      if (item.a === '/app/usuarios') return tienePermisoAdmin(usuario, 'usuarios')
      if (item.a === '/app/seguridad') return tienePermisoAdmin(usuario, 'usuarios.restablecer')
      if (item.a === '/app/cursos') return tienePermisoAdmin(usuario, 'cursos')
      if (item.a === '/app/reportes') return tienePermisoAdmin(usuario, 'reportes')
      if (item.a === '/app/auditoria') return tienePermisoAdmin(usuario, 'auditoria.consultar')
      return false
    })
    : MENU[usuario.rol]
  const etiquetaRol = usuario.rol === 'admin' ? TIPO_ADMIN[usuario.tipoAdmin] ?? TIPO_ADMIN.general : ROL[usuario.rol]

  async function alSalir() {
    await salir()
    navegar('/')
  }

  return (
    <div className="app" data-rol={usuario.rol}>
      <a className="saltar" href="#contenido">Saltar al contenido</a>
      <aside className="lateral">
        <NavLink to="/" className="brand brand-lateral" aria-label="HomeClass, ir al inicio">
          <span className="brand-mark"><i className="bi bi-house-heart-fill" /></span>
          <span><strong className="brand-name">HomeClass</strong><small>{etiquetaRol}</small></span>
        </NavLink>
        <div className="rol-sello"><i className={`bi ${usuario.rol === 'admin' ? 'bi-shield-lock-fill' : usuario.rol === 'docente' ? 'bi-easel2-fill' : 'bi-mortarboard-fill'}`} /><span>{usuario.rol === 'admin' ? 'Control institucional' : usuario.rol === 'docente' ? 'Espacio docente' : 'Espacio estudiantil'}</span></div>

        <nav aria-label="Principal" className="menu">
          {menu.map((m) => (
            <NavLink key={m.a} to={m.a} end={m.fin} className={({ isActive }) => (isActive ? 'menu-item activo' : 'menu-item')}>
              <i className={`bi ${m.i}`} /> <span>{m.t}</span>
            </NavLink>
          ))}
        </nav>

        <div className="lateral-pie">
          <button type="button" className="menu-item" onClick={alternar}>
            <i className={tema === 'dark' ? 'bi bi-sun' : 'bi bi-moon-stars'} /> <span>{tema === 'dark' ? 'Tema claro' : 'Tema oscuro'}</span>
          </button>
          <button type="button" className="menu-item" onClick={alSalir}>
            <i className="bi bi-box-arrow-left" /> <span>Salir</span>
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
