import { Link } from 'react-router-dom'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { Cargando, ErrorCaja, Vacio } from '../components/ui.jsx'

export default function PanelLms() {
  const { usuario } = useAuth()
  const { datos, cargando, error, recargar } = useAsync(() => api.dashboard.obtener(), [])
  const cursos = datos?.cursos ?? []
  const publicados = cursos.filter((curso) => curso.estado?.toLowerCase() === 'publicado').length

  return (
    <>
      {cargando && <Cargando />}
      {error && <ErrorCaja mensaje={error} alReintentar={recargar} />}
      {datos && (
        <>
          <section className="panel-bienvenida">
            <div>
              <p className="panel-kicker">PANEL DE {datos.rol.toUpperCase()}</p>
              <h2>¡Hola, {datos.usuario?.trim()?.split(/\s+/)[0] || usuario.nombre}!</h2>
              <p>Consulta tus cursos y continúa con tu actividad académica.</p>
            </div>
            <Link className="btn btn-primario" to="/app/cursos">
              <i className="bi bi-arrow-up-right-circle" /> {datos.rol === 'admin' ? 'Administrar cursos' : 'Ver mis cursos'}
            </Link>
          </section>

          <dl className="panel-estadisticas" aria-label="Resumen de cursos">
            <div className="panel-estadistica">
              <span className="panel-estadistica-icono panel-icono-azul"><i className="bi bi-journal-bookmark" /></span>
              <dt>{datos.rol === 'admin' ? 'Cursos en catálogo' : 'Mis cursos'}</dt>
              <dd>{datos.totalCursos}</dd>
              <dd className="panel-estadistica-nota">Disponibles para tu cuenta</dd>
            </div>
            <div className="panel-estadistica">
              <span className="panel-estadistica-icono panel-icono-verde"><i className="bi bi-check2-circle" /></span>
              <dt>Cursos publicados</dt>
              <dd>{publicados}</dd>
              <dd className="panel-estadistica-nota">Estado consultado desde el catálogo</dd>
            </div>
            <div className="panel-estadistica">
              <span className="panel-estadistica-icono panel-icono-morado"><i className="bi bi-person-badge" /></span>
              <dt>Sesión activa</dt>
              <dd className="panel-rol-valor">{datos.rol}</dd>
              <dd className="panel-estadistica-nota">{datos.usuario}</dd>
            </div>
          </dl>

          <section className="panel-cursos">
            <div className="panel-seccion-cabecera">
              <div>
                <p className="panel-kicker">TU ESPACIO ACADÉMICO</p>
                <h2>{datos.rol === 'admin' ? 'Cursos del catálogo' : 'Cursos recientes'}</h2>
              </div>
              <Link className="panel-ver-todos" to="/app/cursos">Ver todos <i className="bi bi-arrow-right" /></Link>
            </div>
          {cursos.length === 0 ? (
            <div className="tarjeta">
              <Vacio icono="bi-journal-x" titulo="Sin cursos" texto="Todavía no hay cursos asociados a tu cuenta." />
            </div>
          ) : (
            <div className="rejilla-cursos panel-rejilla-cursos">
              {cursos.slice(0, 6).map((curso) => (
                <article key={curso.id} className="curso-tarjeta">
                  <div className="panel-curso-meta">
                    <span className="panel-curso-icono"><i className="bi bi-journal-text" /></span>
                    <span className={`panel-curso-estado ${curso.estado?.toLowerCase() === 'publicado' ? 'publicado' : ''}`}>{curso.estado}</span>
                  </div>
                  <p className="demo-etiqueta">{curso.codigo}</p>
                  <h3><Link to={`/app/cursos/${curso.id}`}>{curso.nombre}</Link></h3>
                  <p className="suave">{curso.categoria} / {curso.subcategoria}</p>
                  <div className="panel-curso-pie">
                    <span><i className="bi bi-person" /> {curso.docente}</span>
                    <Link to={`/app/cursos/${curso.id}`} aria-label={`Abrir ${curso.nombre}`}><i className="bi bi-arrow-up-right" /></Link>
                  </div>
                </article>
              ))}
            </div>
          )}
          </section>

          <section className="panel-accesos" aria-label="Accesos rápidos">
            <Link className="panel-acceso" to="/app/cursos">
              <span className="panel-acceso-icono"><i className="bi bi-collection" /></span>
              <span><strong>Catálogo de cursos</strong><small>Consulta cursos y su contenido académico.</small></span>
              <i className="bi bi-arrow-right panel-acceso-flecha" />
            </Link>
            {datos.rol === 'admin' && (
              <Link className="panel-acceso" to="/app/categorias">
                <span className="panel-acceso-icono panel-acceso-verde"><i className="bi bi-tags" /></span>
                <span><strong>Categorías</strong><small>Organiza categorías y subcategorías.</small></span>
                <i className="bi bi-arrow-right panel-acceso-flecha" />
              </Link>
            )}
            <Link className="panel-acceso" to="/app/mensajes">
              <span className="panel-acceso-icono panel-acceso-morado"><i className="bi bi-chat-dots" /></span>
              <span><strong>Mensajes privados</strong><small>Comunícate con la comunidad académica.</small></span>
              <i className="bi bi-arrow-right panel-acceso-flecha" />
            </Link>
          </section>
        </>
      )}
    </>
  )
}
