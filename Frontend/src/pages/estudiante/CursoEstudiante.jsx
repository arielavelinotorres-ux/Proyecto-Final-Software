import { useEffect, useState } from 'react'
import { Link, useParams, useSearchParams } from 'react-router-dom'
import { api } from '../../api/index.js'
import { useAviso } from '../../context/ToastContext.jsx'
import useAsync from '../../hooks/useAsync.js'
import SelectorArchivo from '../../components/SelectorArchivo.jsx'
import AvisosCurso from '../../components/AvisosCurso.jsx'
import { Badge, BadgeRiesgo, Cargando, Confirmar, Encabezado, ErrorCaja, StatCard, Vacio } from '../../components/ui.jsx'
import { descargarBlob, listaExtensiones } from '../../utils/archivos.js'
import { fechaCorta, fechaHora, num, tamano, TIPOS_EVALUACION } from '../../utils/formato.js'

export default function CursoEstudiante() {
  const id = Number(useParams().id)
  const [searchParams] = useSearchParams()
  const seccion = searchParams.get('seccion')
  const { datos, cargando, error, recargar } = useAsync(() => api.cursos.miCurso(id), [id])
  const { datos: materiales, error: errorMat } = useAsync(() => api.materiales.listar(id), [id])
  const avisar = useAviso()

  useEffect(() => {
    if (!datos || !['evaluaciones', 'avisos', 'materiales'].includes(seccion)) return
    document.getElementById(`curso-seccion-${seccion}`)?.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }, [datos, seccion])

  if (cargando && !datos) return <Cargando />
  if (error) return <ErrorCaja mensaje={error} alReintentar={recargar} />

  async function descargarMaterial(m) {
    try {
      const { blob, nombre } = await api.materiales.descargar(m.id)
      descargarBlob(blob, nombre)
    } catch (err) { avisar(err.message, 'error') }
  }

  return (
    <>
      <p><Link to="/app/cursos">← Mis cursos</Link></p>
      <Encabezado titulo={datos.curso.nombre} subtitulo={`${datos.curso.codigo} · Docente: ${datos.curso.docente}`} />
      {datos.curso.descripcion && <p>{datos.curso.descripcion}</p>}

      <dl className="stats">
        <StatCard etiqueta="Mi promedio" valor={num(datos.promedio)} icono="bi-bar-chart" />
        <StatCard etiqueta="Promedio del grupo" valor={num(datos.promedioGrupo)} icono="bi-people" />
        <StatCard etiqueta="Mi asistencia" valor={num(datos.asistencia, '%')} icono="bi-calendar-check" />
        <div className="stat"><dt>Mi estado</dt><dd className="dd-badge"><BadgeRiesgo nivel={datos.riesgo} /></dd></div>
      </dl>

      <h2 id="curso-seccion-evaluaciones" className="seccion-titulo">Evaluaciones y entregas</h2>
      <div className="evaluaciones">
        {datos.evaluaciones.map((ev) => <TarjetaEvaluacion key={ev.id} ev={ev} alCambiar={recargar} />)}
      </div>

      <div id="curso-seccion-avisos"><AvisosCurso cursoId={id} /></div>

      <section id="curso-seccion-materiales" className="tarjeta">
        <h2>Materiales del curso</h2>
        {errorMat && <ErrorCaja mensaje={errorMat} />}
        {materiales?.length === 0 && <Vacio icono="bi-folder2" titulo="Sin materiales" texto="Tu docente aún no ha subido archivos." />}
        {materiales?.length > 0 && (
          <ul className="lista">
            {materiales.map((m) => (
              <li key={m.id}>
                <div><strong>{m.titulo}</strong><div className="suave">{m.nombreOriginal} · {tamano(m.tamano)} · {fechaCorta(m.fecha)}</div></div>
                <button type="button" className="btn btn-claro btn-chico" onClick={() => descargarMaterial(m)}><i className="bi bi-download" /> Descargar</button>
              </li>
            ))}
          </ul>
        )}
      </section>
    </>
  )
}

const ESTADOS = {
  calificada: { tono: 'bajo', icono: 'bi-check-circle-fill', texto: 'Calificada' },
  entregada: { tono: 'info', icono: 'bi-cloud-check', texto: 'Entregada' },
  vencida: { tono: 'alto', icono: 'bi-exclamation-octagon-fill', texto: 'Vencida' },
  pendiente: { tono: 'moderado', icono: 'bi-clock', texto: 'Pendiente' },
}

// Una evaluación con su nota y, si pide archivo, el formulario para entregarlo.
function TarjetaEvaluacion({ ev, alCambiar }) {
  const avisar = useAviso()
  const [archivo, setArchivo] = useState(null)
  const [comentario, setComentario] = useState('')
  const [error, setError] = useState(null)
  const [enviando, setEnviando] = useState(false)
  const [retirando, setRetirando] = useState(false)
  const [abierta, setAbierta] = useState(false)

  const estado = ESTADOS[ev.estado]
  const bloqueada = ev.estado === 'calificada'
  const vencida = ev.fecha < new Date().toISOString().slice(0, 10)
  const puedeEntregar = ev.requiereEntrega && !bloqueada && (!vencida || ev.permiteTardia)
  const extensiones = listaExtensiones(ev.extensiones || '')

  async function entregar(e) {
    e.preventDefault()
    if (!archivo) return setError('Elige un archivo para entregar.')
    setEnviando(true)
    setError(null)
    try {
      await api.entregas.entregar(ev.id, archivo, comentario)
      avisar(ev.entrega ? 'Entrega reemplazada.' : 'Entrega enviada.')
      setArchivo(null)
      setComentario('')
      setAbierta(false)
      alCambiar()
    } catch (err) { setError(err.message) } finally { setEnviando(false) }
  }

  async function descargarMia() {
    try {
      const { blob, nombre } = await api.entregas.descargar(ev.entrega.id)
      descargarBlob(blob, nombre)
    } catch (err) { avisar(err.message, 'error') }
  }

  async function retirar() {
    try {
      await api.entregas.retirar(ev.id)
      avisar('Entrega retirada.')
      setRetirando(false)
      alCambiar()
    } catch (err) { avisar(err.message, 'error'); setRetirando(false) }
  }

  return (
    <article className="evaluacion">
      <header className="evaluacion-cab">
        <div>
          <h3>{ev.nombre}</h3>
          <p className="suave">{TIPOS_EVALUACION[ev.tipo]} · vale {ev.peso}% · {ev.requiereEntrega ? 'límite' : 'fecha'}: {fechaCorta(ev.fecha)}</p>
        </div>
        <div className="evaluacion-nota">
          {ev.nota !== null && <strong className="nota-grande" aria-label={`Nota ${ev.nota}`}>{ev.nota}</strong>}
          <Badge tono={estado.tono} icono={estado.icono}>{ev.requiereEntrega || ev.nota !== null ? estado.texto : 'Sin calificar'}</Badge>
        </div>
      </header>

      {ev.instrucciones && <p className="instrucciones">{ev.instrucciones}</p>}

      {ev.requiereEntrega && (
        <div className="entrega">
          {ev.entrega ? (
            <div className="entrega-actual">
              <p>
                <i className="bi bi-paperclip" /> <strong>{ev.entrega.nombreOriginal}</strong> <small className="suave">({tamano(ev.entrega.tamano)}) · {fechaHora(ev.entrega.fecha)}</small>
                {ev.entrega.tardia && <> <Badge tono="moderado" icono="bi-clock-history">Tardía</Badge></>}
              </p>
              {ev.entrega.comentario && <p className="suave comentario">“{ev.entrega.comentario}”</p>}
              <div className="acciones-linea">
                <button type="button" className="btn btn-claro btn-chico" onClick={descargarMia}><i className="bi bi-download" /> Descargar</button>
                {puedeEntregar && <button type="button" className="btn btn-claro btn-chico" onClick={() => setAbierta((a) => !a)}>Reemplazar</button>}
                {!bloqueada && <button type="button" className="btn btn-peligro-claro btn-chico" onClick={() => setRetirando(true)}>Retirar</button>}
              </div>
              {bloqueada && <p className="suave">Ya fue calificada: la entrega no se puede cambiar.</p>}
            </div>
          ) : puedeEntregar ? (
            !abierta && <button type="button" className="btn btn-primario" onClick={() => setAbierta(true)}><i className="bi bi-cloud-arrow-up" /> Entregar archivo</button>
          ) : (
            <p className="msg msg-error">El plazo de entrega terminó.</p>
          )}

          {abierta && puedeEntregar && (
            <form onSubmit={entregar} className="form-modal">
              <SelectorArchivo archivo={archivo} alElegir={setArchivo} alError={setError} extensiones={extensiones} maxMb={ev.maxMb} />
              <div className="campo">
                <label htmlFor={`com-${ev.id}`}>Comentario para tu docente (opcional)</label>
                <textarea id={`com-${ev.id}`} rows={2} value={comentario} onChange={(e) => setComentario(e.target.value)} />
              </div>
              {vencida && <p className="msg msg-error">Se registrará como entrega tardía.</p>}
              {error && <p className="msg msg-error" role="alert">{error}</p>}
              <div className="acciones">
                <button type="button" className="btn btn-claro" onClick={() => { setAbierta(false); setArchivo(null); setError(null) }}>Cancelar</button>
                <button type="submit" className="btn btn-primario" disabled={enviando || !archivo}>{enviando ? 'Enviando…' : ev.entrega ? 'Reemplazar entrega' : 'Enviar entrega'}</button>
              </div>
            </form>
          )}
        </div>
      )}

      <Confirmar abierto={retirando} titulo="Retirar entrega" textoBoton="Retirar" texto="Se borrará el archivo que subiste. Podrás entregar otro si el plazo sigue abierto." alConfirmar={retirar} alCancelar={() => setRetirando(false)} />
    </article>
  )
}
