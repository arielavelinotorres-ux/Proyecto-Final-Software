import { Link } from 'react-router-dom'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { Cargando, Encabezado, ErrorCaja, Vacio } from '../components/ui.jsx'

const ACCIONES = {
  docente: [
    ['notas', 'Calificaciones', 'bi-journal-check'],
    ['asistencia', 'Asistencia', 'bi-calendar-check'],
    ['evaluaciones', 'Evaluaciones', 'bi-clipboard2-check'],
    ['entregas', 'Entregas', 'bi-inbox'],
    ['materiales', 'Materiales', 'bi-folder2-open'],
    ['avisos', 'Avisos', 'bi-megaphone'],
  ],
  estudiante: [
    ['evaluaciones', 'Evaluaciones y entregas', 'bi-journal-check'],
    ['materiales', 'Materiales', 'bi-folder2-open'],
    ['avisos', 'Avisos del curso', 'bi-megaphone'],
  ],
}

export default function CentroAcademico() {
  const { usuario } = useAuth()
  const docente = usuario.rol === 'docente'
  const { datos, cargando, error, recargar } = useAsync(() => api.cursos.listar(), [])

  return (
    <>
      <Encabezado
        titulo={docente ? 'Herramientas docentes' : 'Actividad académica'}
        subtitulo={docente ? 'Accede a las tareas de gestión de cada curso.' : 'Continúa tus cursos desde evaluaciones, materiales y avisos.'}
      />
      {cargando && <Cargando />}
      {error && <ErrorCaja mensaje={error} alReintentar={recargar} />}
      {datos?.length === 0 && <Vacio icono="bi-journal-x" titulo="Sin cursos" texto="Cuando tengas cursos asignados, sus accesos aparecerán aquí." />}
      {datos?.length > 0 && (
        <div className="rejilla-cursos">
          {datos.map((curso) => (
            <article className="curso-tarjeta" key={curso.id}>
              <p className="demo-etiqueta">{curso.codigo} · {curso.periodo}</p>
              <h2><Link to={`/app/cursos/${curso.id}`}>{curso.nombre}</Link></h2>
              <p className="suave">{docente ? `${curso.estudiantes} estudiantes` : `Docente: ${curso.docente}`}</p>
              <nav className="atajos-curso-enlaces" aria-label={`Accesos de ${curso.nombre}`}>
                {ACCIONES[usuario.rol].map(([seccion, etiqueta, icono]) => (
                  <Link key={seccion} to={`/app/cursos/${curso.id}?seccion=${seccion}`}>
                    <i className={`bi ${icono}`} /> {etiqueta}
                  </Link>
                ))}
              </nav>
            </article>
          ))}
        </div>
      )}
    </>
  )
}