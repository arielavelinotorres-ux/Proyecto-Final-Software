import { useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { BadgeRiesgo, BarList, Cargando, Encabezado, ErrorCaja, StatCard, Vacio, Badge } from '../components/ui.jsx'
import { fechaCorta, num, TIPOS_EVALUACION } from '../utils/formato.js'

// Una misma ruta (/app) muestra un balance distinto según quién entró.
export default function Resumen() {
  const { usuario } = useAuth()
  const { datos, cargando, error, recargar } = useAsync(() => api.dashboard.obtener(), [])

  if (cargando) return <Cargando />
  if (error) return <ErrorCaja mensaje={error} alReintentar={recargar} />

  return (
    <>
      <Encabezado titulo={`Hola, ${usuario.nombre.split(' ')[0]}`} subtitulo="Este es tu balance de hoy." />
      {datos.rol === 'admin' && <ResumenAdmin d={datos} />}
      {datos.rol === 'docente' && <ResumenDocente d={datos} />}
      {datos.rol === 'estudiante' && <ResumenEstudiante d={datos} />}
    </>
  )
}

function ResumenAdmin({ d }) {
  const totalRiesgo = d.riesgo.alto + d.riesgo.moderado + d.riesgo.bajo
  const [metrica, setMetrica] = useState('promedio')
  const [buscar, setBuscar] = useState('')
  const [orden, setOrden] = useState('nombre')
  const [cursoId, setCursoId] = useState(null)
  const cursos = d.cursos
    .filter((curso) => curso.nombre.toLowerCase().includes(buscar.trim().toLowerCase()))
    .sort((a, b) => orden === 'nombre' ? a.nombre.localeCompare(b.nombre) : (b[orden] ?? -1) - (a[orden] ?? -1))
  const cursoSeleccionado = cursos.find((curso) => curso.id === cursoId)

  return (
    <>
      <div className="dashboard-intro">
        <div><span className="dashboard-kicker">Vista institucional</span><h2>Un panorama claro para decidir el siguiente paso.</h2></div>
        <div className="dashboard-acciones">
          <Link className="btn btn-claro btn-chico" to="/app/usuarios"><i className="bi bi-people" /> Usuarios</Link>
          <Link className="btn btn-primario btn-chico" to="/app/cursos"><i className="bi bi-journal-plus" /> Gestionar cursos</Link>
        </div>
      </div>
      <dl className="stats">
        <StatCard etiqueta="Cuentas" valor={d.usuarios.total} nota={`${d.usuarios.docentes} docentes · ${d.usuarios.estudiantes} estudiantes`} icono="bi-people" />
        <StatCard etiqueta="Cursos" valor={d.cursos.length} icono="bi-journal-bookmark" />
        <StatCard etiqueta="Promedio general" valor={num(d.promedioGeneral)} icono="bi-bar-chart" />
        <StatCard etiqueta="Asistencia general" valor={num(d.asistenciaGeneral, '%')} icono="bi-calendar-check" />
      </dl>
      <div className="dashboard-grid">
        <section className="tarjeta dashboard-panel">
          <header className="dashboard-panel-cabecera">
            <div><h2>Desempeño por curso</h2><p className="suave">Selecciona una fila para ver sus indicadores.</p></div>
            <SelectorMetrica metrica={metrica} alCambiar={setMetrica} />
          </header>
          <div className="dashboard-filtros">
            <label><span>Buscar curso</span><input type="search" value={buscar} onChange={(e) => setBuscar(e.target.value)} placeholder="Nombre del curso" /></label>
            <label><span>Ordenar por</span><select value={orden} onChange={(e) => setOrden(e.target.value)}><option value="nombre">Nombre</option><option value="promedio">Promedio</option><option value="asistencia">Asistencia</option></select></label>
          </div>
          <BarrasCursos cursos={cursos} metrica={metrica} seleccionado={cursoId} alSeleccionar={setCursoId} />
          {cursoSeleccionado && <div className="dashboard-seleccion">
            <div><strong>{cursoSeleccionado.nombre}</strong><span>{cursoSeleccionado.codigo} · {cursoSeleccionado.periodo}</span></div>
            <div className="dashboard-seleccion-metricas"><span>Promedio <b>{num(cursoSeleccionado.promedio)}</b></span><span>Asistencia <b>{num(cursoSeleccionado.asistencia, '%')}</b></span><span>Estudiantes <b>{cursoSeleccionado.estudiantes}</b></span></div>
            <Link className="btn btn-claro btn-chico" to={`/app/cursos/${cursoSeleccionado.id}`}>Abrir curso <i className="bi bi-arrow-up-right" /></Link>
          </div>}
        </section>
        <section className="tarjeta dashboard-panel dashboard-resumen-lateral">
          <div className="dashboard-panel-cabecera"><div><h2>Panorama de la comunidad</h2><p className="suave">Distribución de cuentas activas.</p></div><i className="bi bi-people dashboard-panel-icono" /></div>
          <div className="dashboard-cuentas">
            <span><i className="bi bi-shield-check" /> Administradores <b>{d.usuarios.admins}</b></span>
            <span><i className="bi bi-easel2" /> Docentes <b>{d.usuarios.docentes}</b></span>
            <span><i className="bi bi-mortarboard" /> Estudiantes <b>{d.usuarios.estudiantes}</b></span>
          </div>
          <div className="dashboard-riesgo-cab"><div><h3>Seguimiento académico</h3><p className="suave">{totalRiesgo} matrículas con datos</p></div><span className="dashboard-total-riesgo">{d.riesgo.alto}<small> alto</small></span></div>
          <BarList max={Math.max(totalRiesgo, 1)} filas={[
            { etiqueta: 'Riesgo alto', valor: d.riesgo.alto, tono: 'alto' },
            { etiqueta: 'Riesgo moderado', valor: d.riesgo.moderado, tono: 'moderado' },
            { etiqueta: 'Al día', valor: d.riesgo.bajo, tono: 'bajo' },
          ]} />
          {d.usuarios.inactivos > 0 && <p className="dashboard-nota"><i className="bi bi-info-circle" /> {d.usuarios.inactivos} cuentas inactivas.</p>}
        </section>
      </div>
    </>
  )
}

function ResumenDocente({ d }) {
  const [metrica, setMetrica] = useState('promedio')
  const [cursoId, setCursoId] = useState('todos')
  const cursos = cursoId === 'todos' ? d.cursos : d.cursos.filter((curso) => String(curso.id) === cursoId)
  const alertas = cursoId === 'todos' ? d.alertas : d.alertas.filter((alerta) => String(alerta.cursoId) === cursoId)
  const proximas = cursoId === 'todos' ? d.proximas : d.proximas.filter((evaluacion) => String(evaluacion.cursoId) === cursoId)
  const promedio = cursoId === 'todos' ? d.promedioGeneral : cursos[0]?.promedio

  return (
    <>
      <div className="dashboard-intro">
        <div><span className="dashboard-kicker">Espacio docente</span><h2>Tus cursos, pendientes y alertas de un vistazo.</h2></div>
        <label className="dashboard-filtro-curso"><span>Ver curso</span><select value={cursoId} onChange={(e) => setCursoId(e.target.value)}><option value="todos">Todos mis cursos</option>{d.cursos.map((curso) => <option key={curso.id} value={curso.id}>{curso.nombre}</option>)}</select></label>
      </div>
      <dl className="stats">
        <StatCard etiqueta="Mis cursos" valor={d.cursos.length} icono="bi-easel" />
        <StatCard etiqueta="Entregas por revisar" valor={d.porRevisar} icono="bi-inbox" tono={d.porRevisar > 0 ? 'atencion' : undefined} />
        <StatCard etiqueta="En riesgo alto" valor={d.enRiesgoAlto} icono="bi-exclamation-octagon" tono={d.enRiesgoAlto > 0 ? 'alto' : undefined} />
        <StatCard etiqueta={cursoId === 'todos' ? 'Promedio de mis cursos' : 'Promedio del curso'} valor={num(promedio)} icono="bi-bar-chart" />
      </dl>
      <div className="dashboard-grid">
        <section className="tarjeta dashboard-panel">
          <header className="dashboard-panel-cabecera"><div><h2>Progreso de cursos</h2><p className="suave">Cambia la métrica o selecciona un curso.</p></div><SelectorMetrica metrica={metrica} alCambiar={setMetrica} /></header>
          <BarrasCursos cursos={cursos} metrica={metrica} seleccionado={cursoId === 'todos' ? null : Number(cursoId)} alSeleccionar={(id) => setCursoId(id === null ? 'todos' : String(id))} />
          {cursoId !== 'todos' && <button type="button" className="dashboard-quitar-filtro" onClick={() => setCursoId('todos')}><i className="bi bi-arrow-counterclockwise" /> Ver todos los cursos</button>}
        </section>
        <section className="tarjeta dashboard-panel">
          <header className="dashboard-panel-cabecera"><div><h2>Alertas de estudiantes</h2><p className="suave">Casos que conviene revisar.</p></div><span className="dashboard-panel-icono"><i className="bi bi-activity" /></span></header>
          {alertas.length === 0 ? (
            <Vacio icono="bi-emoji-smile" titulo="Todo en orden" texto="Ningún estudiante está en riesgo ahora." />
          ) : (
            <ul className="lista dashboard-lista">
              {alertas.map((a) => (
                <li key={`${a.estudianteId}-${a.cursoId}`}>
                  <div>
                    <strong>{a.nombre}</strong>
                    <small className="suave"> · <Link to={`/app/cursos/${a.cursoId}?seccion=notas`}>{a.curso}</Link></small>
                    <div className="suave">Promedio {num(a.promedio)} · Asistencia {num(a.asistencia, '%')}</div>
                  </div>
                  <BadgeRiesgo nivel={a.riesgo} />
                </li>
              ))}
            </ul>
          )}
        </section>
        <section className="tarjeta dashboard-panel dashboard-panel-ancho">
          <header className="dashboard-panel-cabecera"><div><h2>Próximas evaluaciones</h2><p className="suave">Planifica lo que viene en tus cursos.</p></div><Link className="btn btn-claro btn-chico" to="/app/agenda">Abrir agenda <i className="bi bi-arrow-right" /></Link></header>
          {proximas.length === 0 ? (
            <Vacio icono="bi-calendar2-check" titulo="Sin evaluaciones próximas" />
          ) : (
            <ul className="lista dashboard-proximas">
              {proximas.map((p) => (
                <li key={p.evaluacionId}>
                  <div>
                    <strong>{p.nombre}</strong>
                    <div className="suave"><Link to={`/app/cursos/${p.cursoId}?seccion=evaluaciones`}>{p.curso}</Link> · {TIPOS_EVALUACION[p.tipo]}</div>
                  </div>
                  <Badge icono="bi-calendar-event">{fechaCorta(p.fecha)}</Badge>
                </li>
              ))}
            </ul>
          )}
        </section>
      </div>
    </>
  )
}

function SelectorMetrica({ metrica, alCambiar }) {
  return (
    <div className="dashboard-selector-metrica" role="group" aria-label="Métrica del gráfico">
      <button type="button" aria-pressed={metrica === 'promedio'} className={metrica === 'promedio' ? 'activo' : ''} onClick={() => alCambiar('promedio')}>Promedio</button>
      <button type="button" aria-pressed={metrica === 'asistencia'} className={metrica === 'asistencia' ? 'activo' : ''} onClick={() => alCambiar('asistencia')}>Asistencia</button>
    </div>
  )
}

function BarrasCursos({ cursos, metrica, seleccionado, alSeleccionar }) {
  if (cursos.length === 0) return <Vacio icono="bi-journal-x" titulo="Sin cursos para mostrar" texto="Ajusta la búsqueda o revisa tus cursos asignados." />
  const etiqueta = metrica === 'promedio' ? 'promedio' : 'asistencia'
  return (
    <ul className="dashboard-barras" aria-label={`Cursos comparados por ${etiqueta}`}>
      {cursos.map((curso) => {
        const valor = curso[metrica]
        const porcentaje = Number.isFinite(Number(valor)) ? Math.max(0, Math.min(100, Number(valor))) : 0
        const activo = seleccionado === curso.id
        return (
          <li key={curso.id}>
            <button type="button" className={activo ? 'dashboard-barra activo' : 'dashboard-barra'} aria-pressed={activo} onClick={() => alSeleccionar(activo ? null : curso.id)}>
              <span className="dashboard-barra-nombre">{curso.nombre}</span>
              <span className="dashboard-barra-pista" aria-hidden="true"><span className={metrica === 'asistencia' ? 'dashboard-barra-relleno asistencia' : 'dashboard-barra-relleno'} style={{ width: `${porcentaje}%` }} /></span>
              <strong>{num(valor, metrica === 'asistencia' ? '%' : '')}</strong>
            </button>
          </li>
        )
      })}
    </ul>
  )
}

function ResumenEstudiante({ d }) {
  return (
    <>
      <dl className="stats">
        <StatCard etiqueta="Mi promedio" valor={num(d.promedio)} icono="bi-bar-chart" />
        <StatCard etiqueta="Mi asistencia" valor={num(d.asistencia, '%')} icono="bi-calendar-check" />
        <StatCard etiqueta="Entregas pendientes" valor={d.pendientes.length} icono="bi-cloud-arrow-up" tono={d.pendientes.length > 0 ? 'atencion' : undefined} />
        <StatCard etiqueta="Cursos" valor={d.cursos.length} icono="bi-mortarboard" />
      </dl>
      <div className="dos-col">
        <section className="tarjeta">
          <h2>Por entregar</h2>
          {d.pendientes.length === 0 ? (
            <Vacio icono="bi-emoji-smile" titulo="Estás al día" texto="No tienes entregas pendientes." />
          ) : (
            <ul className="lista">
              {d.pendientes.map((p) => (
                <li key={p.evaluacionId}>
                  <div>
                    <strong>{p.nombre}</strong>
                    <div className="suave"><Link to={`/app/cursos/${p.cursoId}`}>{p.curso}</Link> · {TIPOS_EVALUACION[p.tipo]}</div>
                  </div>
                  <Badge tono={p.vencida ? 'alto' : 'moderado'} icono={p.vencida ? 'bi-exclamation-octagon-fill' : 'bi-clock'}>
                    {p.vencida ? 'Venció' : 'Vence'} {fechaCorta(p.fecha)}
                  </Badge>
                </li>
              ))}
            </ul>
          )}
        </section>
        <section className="tarjeta">
          <h2>Mi promedio por curso</h2>
          <BarList filas={d.cursos.map((c) => ({ etiqueta: c.nombre, valor: c.promedio }))} />
          <h3 className="sub">Promedio del grupo</h3>
          <BarList filas={d.cursos.map((c) => ({ etiqueta: c.nombre, valor: c.promedioGrupo, tono: 'neutro' }))} />
        </section>
      </div>
    </>
  )
}
