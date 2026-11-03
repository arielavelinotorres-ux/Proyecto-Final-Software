import { Link } from 'react-router-dom'
import { api } from '../api/index.js'
import useAsync from '../hooks/useAsync.js'
import { Cargando, Encabezado, ErrorCaja, Vacio } from '../components/ui.jsx'

export default function PanelLms() {
  const { datos, cargando, error, recargar } = useAsync(() => api.dashboard.obtener(), [])

  return (
    <>
      <Encabezado titulo="Resumen" subtitulo="Información consultada desde la base de datos a través del backend." />
      {cargando && <Cargando />}
      {error && <ErrorCaja mensaje={error} alReintentar={recargar} />}
      {datos && (
        <>
          <section className="rejilla" aria-label="Resumen">
            <article className="feature">
              <span className="feature-icono"><i className="bi bi-person-circle" /></span>
              <h2>Sesión activa</h2>
              <p>{datos.usuario} · {datos.rol}</p>
            </article>
            <article className="feature">
              <span className="feature-icono"><i className="bi bi-journal-bookmark" /></span>
              <h2>Cursos disponibles</h2>
              <p>{datos.totalCursos}</p>
            </article>
          </section>
          <Encabezado titulo="Cursos" subtitulo="Cursos disponibles según tu perfil y matrícula." />
          {datos.cursos.length === 0 ? (
            <Vacio icono="bi-journal-x" titulo="Sin cursos" texto="Todavía no hay cursos asociados a tu cuenta." />
          ) : (
            <div className="rejilla-cursos">
              {datos.cursos.map((curso) => (
                <article key={curso.id} className="curso-tarjeta">
                  <p className="demo-etiqueta">{curso.codigo} · {curso.estado}</p>
                  <h2><Link to={`/app/cursos/${curso.id}`}>{curso.nombre}</Link></h2>
                  <p className="suave">{curso.categoria} / {curso.subcategoria}</p>
                  <p className="suave">Docente: {curso.docente} · {curso.estudiantes} estudiantes</p>
                </article>
              ))}
            </div>
          )}
        </>
      )}
    </>
  )
}
