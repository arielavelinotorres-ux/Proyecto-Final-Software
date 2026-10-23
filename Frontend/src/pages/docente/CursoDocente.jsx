import { useState } from 'react'
import { Link, useParams, useSearchParams } from 'react-router-dom'
import { api } from '../../api/index.js'
import useAsync from '../../hooks/useAsync.js'
import { Cargando, Encabezado, ErrorCaja, StatCard, Tabs } from '../../components/ui.jsx'
import { num } from '../../utils/formato.js'
import Libro from './Libro.jsx'
import Asistencia from './Asistencia.jsx'
import Evaluaciones from './Evaluaciones.jsx'
import Entregas from './Entregas.jsx'
import Materiales from './Materiales.jsx'
import AvisosCurso from '../../components/AvisosCurso.jsx'

const PESTANAS = [
  { id: 'notas', etiqueta: 'Calificaciones', icono: 'bi-journal-check' },
  { id: 'asistencia', etiqueta: 'Asistencia', icono: 'bi-calendar-check' },
  { id: 'evaluaciones', etiqueta: 'Evaluaciones', icono: 'bi-clipboard2-check' },
  { id: 'entregas', etiqueta: 'Entregas', icono: 'bi-inbox' },
  { id: 'materiales', etiqueta: 'Materiales', icono: 'bi-folder2-open' },
  { id: 'avisos', etiqueta: 'Avisos', icono: 'bi-megaphone' },
]

export default function CursoDocente() {
  const id = Number(useParams().id)
  const [searchParams] = useSearchParams()
  const { datos: curso, cargando, error, recargar } = useAsync(() => api.cursos.obtener(id), [id])
  const { datos: resumen, recargar: recargarResumen } = useAsync(() => api.cursos.listar().then((l) => l.find((c) => c.id === id)), [id])
  const [pestana, setPestana] = useState(() => PESTANAS.some((p) => p.id === searchParams.get('seccion')) ? searchParams.get('seccion') : 'notas')
  const [evaluacionId, setEvaluacionId] = useState(null) // para saltar de Evaluaciones a Entregas

  if (cargando && !curso) return <Cargando />
  if (error) return <ErrorCaja mensaje={error} alReintentar={recargar} />

  function verEntregas(evId) {
    setEvaluacionId(evId)
    setPestana('entregas')
  }

  return (
    <>
      <p><Link to="/app/cursos">← Mis cursos</Link></p>
      <Encabezado titulo={curso.nombre} subtitulo={`${curso.codigo} · ${curso.periodo}`} />
      <dl className="stats">
        <StatCard etiqueta="Estudiantes" valor={curso.estudiantes.length} icono="bi-people" />
        <StatCard etiqueta="Promedio" valor={num(resumen?.promedio)} icono="bi-bar-chart" />
        <StatCard etiqueta="Asistencia" valor={num(resumen?.asistencia, '%')} icono="bi-calendar-check" />
        <StatCard etiqueta="En riesgo alto" valor={resumen?.enRiesgo ?? '—'} icono="bi-exclamation-octagon" tono={resumen?.enRiesgo > 0 ? 'alto' : undefined} />
      </dl>

      <Tabs items={PESTANAS} activo={pestana} alCambiar={setPestana} etiqueta="Secciones del curso" />
      <div role="tabpanel" aria-labelledby={`tab-${pestana}`}>
        {pestana === 'notas' && <Libro cursoId={id} alCambiar={recargarResumen} />}
        {pestana === 'asistencia' && <Asistencia cursoId={id} alGuardar={recargarResumen} />}
        {pestana === 'evaluaciones' && <Evaluaciones cursoId={id} alVerEntregas={verEntregas} alCambiar={recargarResumen} />}
        {pestana === 'entregas' && <Entregas cursoId={id} evaluacionInicial={evaluacionId} alCalificar={recargarResumen} />}
        {pestana === 'materiales' && <Materiales cursoId={id} />}
        {pestana === 'avisos' && <AvisosCurso cursoId={id} puedePublicar />}
      </div>
    </>
  )
}
