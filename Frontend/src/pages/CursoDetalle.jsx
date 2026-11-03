import { useEffect, useMemo, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import { useAviso } from '../context/ToastContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { Cargando, Confirmar, Encabezado, ErrorCaja, Vacio } from '../components/ui.jsx'

export default function CursoDetalle() {
  const { id: idTexto } = useParams()
  const id = Number(idTexto)
  const { usuario } = useAuth()
  const avisar = useAviso()
  const esDocente = usuario.rol === 'docente'
  const esAdmin = usuario.rol === 'admin'
  const { datos: curso, cargando, error, recargar } = useAsync(() => api.cursos.obtener(id), [id])
  const { datos: secciones, error: errorContenido } = useAsync(() => api.cursos.contenido(id), [id])
  const { datos: foros, error: errorForos, recargar: recargarForos } = useAsync(() => api.cursos.foros(id), [id])
  const { datos: estudiantes, error: errorEstudiantes, recargar: recargarEstudiantes } = useAsync(
    () => esAdmin ? api.usuarios.listar('estudiante') : Promise.resolve([]),
    [esAdmin],
  )
  const [foroSeleccionado, setForoSeleccionado] = useState(null)
  const [publicaciones, setPublicaciones] = useState([])
  const [progreso, setProgreso] = useState({})
  const [cargandoPublicaciones, setCargandoPublicaciones] = useState(false)
  const [texto, setTexto] = useState('')
  const [nuevoForo, setNuevoForo] = useState(false)
  const [foroTitulo, setForoTitulo] = useState('')
  const [foroDescripcion, setForoDescripcion] = useState('')
  const [foroAArchivar, setForoAArchivar] = useState(null)
  const [estudianteId, setEstudianteId] = useState('')
  const [errorAccion, setErrorAccion] = useState(null)
  const idsMatriculados = useMemo(() => new Set((curso?.estudiantes ?? []).map((item) => item.id)), [curso])
  const disponibles = (estudiantes ?? []).filter((item) => !idsMatriculados.has(item.id))

  useEffect(() => {
    if (!secciones) return
    const inicial = {}
    secciones.forEach((seccion) => (seccion.lecciones ?? []).forEach((leccion) => {
      inicial[leccion.id] = Boolean(leccion.completado)
    }))
    setProgreso(inicial)
  }, [secciones])

  useEffect(() => {
    if (!foroSeleccionado) {
      setPublicaciones([])
      return
    }
    let vigente = true
    setCargandoPublicaciones(true)
    api.foros.publicaciones(foroSeleccionado)
      .then((rows) => vigente && setPublicaciones(rows))
      .catch((err) => vigente && setErrorAccion(err.message))
      .finally(() => vigente && setCargandoPublicaciones(false))
    return () => { vigente = false }
  }, [foroSeleccionado])

  async function publicar(event) {
    event.preventDefault()
    if (!texto.trim()) return
    try {
      await api.foros.publicar(foroSeleccionado, { contenido: texto })
      setTexto('')
      setPublicaciones(await api.foros.publicaciones(foroSeleccionado))
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function crearForo(event) {
    event.preventDefault()
    try {
      await api.cursos.crearForo(id, { titulo: foroTitulo, mensaje: foroDescripcion })
      setForoTitulo('')
      setForoDescripcion('')
      setNuevoForo(false)
      recargarForos()
      avisar('Foro creado.')
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function cambiarEstadoForo(foro) {
    const estado = foro.estado === 'Abierto' ? 'Cerrado' : 'Abierto'
    try {
      await api.cursos.actualizarForo(foro.id, { titulo: foro.titulo, descripcion: foro.descripcion, estado })
      recargarForos()
      avisar(`Foro ${estado.toLowerCase()}.`)
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function archivarForo() {
    try {
      await api.cursos.archivarForo(foroAArchivar.id)
      setForoSeleccionado(null)
      setForoAArchivar(null)
      recargarForos()
      avisar('Foro archivado.')
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function matricular(event) {
    event.preventDefault()
    try {
      await api.cursos.matricular(id, Number(estudianteId))
      setEstudianteId('')
      avisar('Estudiante matriculado.')
      recargar()
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function desmatricular(estudiante) {
    try {
      await api.cursos.desmatricular(id, estudiante.id)
      avisar('Matrícula eliminada.')
      recargar()
      recargarEstudiantes()
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function cambiarEstadoMatricula(estudiante, estado) {
    try {
      await api.cursos.actualizarMatricula(id, estudiante.id, estado)
      avisar('Estado de matrícula actualizado.')
      recargar()
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function actualizarProgreso(leccionId) {
    const completado = !progreso[leccionId]
    try {
      await api.cursos.actualizarProgreso(leccionId, completado)
      setProgreso((actual) => ({ ...actual, [leccionId]: completado }))
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  function urlRecursoSegura(url) {
    return /^https?:\/\//i.test(url) || url.startsWith('/')
  }

  if (cargando) return <Cargando />
  if (error) return <ErrorCaja mensaje={error} alReintentar={recargar} />
  if (!curso) return null

  return (
    <>
      <p><Link to="/app/cursos">← Cursos</Link></p>
      <Encabezado titulo={curso.nombre} subtitulo={`${curso.codigo} · ${curso.estado} · ${curso.categoria} / ${curso.subcategoria}`} />
      {curso.descripcion && <p>{curso.descripcion}</p>}

      {esAdmin && (
        <section className="tarjeta">
          <h2>Matrículas ({curso.estudiantes.length})</h2>
          {errorEstudiantes && <ErrorCaja mensaje={errorEstudiantes} alReintentar={recargarEstudiantes} />}
          <form className="fila-form" onSubmit={matricular}>
            <select aria-label="Estudiante" value={estudianteId} onChange={(event) => setEstudianteId(event.target.value)} required>
              <option value="">Selecciona un estudiante</option>
              {disponibles.map((item) => <option key={item.id} value={item.id}>{item.nombre} · {item.correo}</option>)}
            </select>
            <button type="submit" className="btn btn-primario" disabled={!estudianteId}>Matricular</button>
          </form>
          <ul className="lista">
            {curso.estudiantes.map((estudiante) => (
              <li key={estudiante.id}>
                <span>{estudiante.nombre} · {estudiante.correo}</span>
                <div className="acciones-linea">
                  <select aria-label={`Estado de matrícula de ${estudiante.nombre}`} value={estudiante.estado} onChange={(event) => cambiarEstadoMatricula(estudiante, event.target.value)}>
                    <option value="Activo">Activo</option>
                    <option value="Completado">Completado</option>
                    <option value="Cancelado">Cancelado</option>
                  </select>
                  <button type="button" className="btn btn-peligro-claro btn-chico" onClick={() => desmatricular(estudiante)}>Eliminar matrícula</button>
                </div>
              </li>
            ))}
          </ul>
        </section>
      )}

      <section className="tarjeta">
        <Encabezado titulo="Contenido del curso" subtitulo="Secciones y lecciones guardadas en la base de datos." />
        {errorContenido && <ErrorCaja mensaje={errorContenido} />}
        {secciones?.length === 0 && <Vacio titulo="Sin contenido" texto="Este curso aún no tiene secciones o lecciones." />}
        {secciones?.map((seccion) => (
          <details className="curso-seccion" key={seccion.id} open>
            <summary><strong>{seccion.titulo}</strong></summary>
            {(seccion.lecciones ?? []).length === 0
              ? <p className="suave">Sin lecciones en esta sección.</p>
              : <ul className="lista">{seccion.lecciones.map((leccion) => (
                <li key={leccion.id}>
                  <div>
                    <strong>{leccion.titulo}</strong>
                    <p className="suave">{leccion.duracionMinutos} min{leccion.esGratuita ? ' · Gratuita' : ''}</p>
                    {leccion.contenido && <p>{leccion.contenido}</p>}
                    {leccion.urlVideo && urlRecursoSegura(leccion.urlVideo) && <a href={leccion.urlVideo} target="_blank" rel="noreferrer">Ver video</a>}
                    {usuario.rol === 'estudiante' && <label className="progreso-leccion">
                      <input type="checkbox" checked={Boolean(progreso[leccion.id])} onChange={() => actualizarProgreso(leccion.id)} />
                      Lección completada
                    </label>}
                    {leccion.recursos?.length > 0 && <ul className="lista">{leccion.recursos.map((recurso) => (
                      <li key={recurso.id}>
                        {urlRecursoSegura(recurso.urlRecurso)
                          ? <a href={recurso.urlRecurso} target="_blank" rel="noreferrer">{recurso.nombre}</a>
                          : <span>{recurso.nombre}</span>}
                        <small className="suave">{recurso.tipoArchivo}</small>
                      </li>
                    ))}</ul>}
                  </div>
                </li>
              ))}</ul>}
          </details>
        ))}
      </section>

      <section className="tarjeta">
        <Encabezado titulo="Foros" subtitulo="Conversaciones persistidas en Comunicacion.foros y publicaciones_foro.">
          {esDocente && <button className="btn btn-primario" type="button" onClick={() => setNuevoForo((value) => !value)}>Nuevo foro</button>}
        </Encabezado>
        {nuevoForo && (
          <form className="form" onSubmit={crearForo}>
            <div className="campo"><label htmlFor="foro-titulo">Título</label><input id="foro-titulo" value={foroTitulo} onChange={(event) => setForoTitulo(event.target.value)} maxLength="150" required /></div>
            <div className="campo"><label htmlFor="foro-descripcion">Descripción (máximo 100 caracteres)</label><textarea id="foro-descripcion" value={foroDescripcion} onChange={(event) => setForoDescripcion(event.target.value)} maxLength="100" required /></div>
            <button className="btn btn-primario" type="submit">Crear foro</button>
          </form>
        )}
        {errorForos && <ErrorCaja mensaje={errorForos} alReintentar={recargarForos} />}
        {foros?.length === 0 && <Vacio titulo="Sin foros" texto="Todavía no hay foros para este curso." />}
        <div className="lista-foros">
          {foros?.map((foro) => (
            <div key={foro.id} className="foro-card">
              <button className="foro-item" type="button" onClick={() => { setForoSeleccionado(foro.id); setErrorAccion(null) }}>
                <strong>{foro.titulo}</strong><span>{foro.descripcion}</span><small>{foro.publicaciones} publicaciones · {foro.estado}</small>
              </button>
              {esDocente && <div className="acciones-linea">
                <button className="btn btn-claro btn-chico" type="button" onClick={() => cambiarEstadoForo(foro)}>{foro.estado === 'Abierto' ? 'Cerrar foro' : 'Reabrir foro'}</button>
                <button className="btn btn-peligro-claro btn-chico" type="button" onClick={() => setForoAArchivar(foro)}>Archivar</button>
              </div>}
            </div>
          ))}
        </div>
        {foroSeleccionado && (
          <div className="foro-contenido">
            <h3>{foros?.find((foro) => foro.id === foroSeleccionado)?.titulo}</h3>
            {cargandoPublicaciones && <Cargando texto="Cargando publicaciones…" />}
            {publicaciones.map((publicacion) => (
              <article className="publicacion" key={publicacion.id}>
                <p>{publicacion.contenido}</p>
                <small>{publicacion.autor} · {new Date(publicacion.creadoEn).toLocaleString()}</small>
              </article>
            ))}
            <form className="form" onSubmit={publicar}>
              <div className="campo"><label htmlFor="publicacion">Escribe una publicación</label><textarea id="publicacion" value={texto} onChange={(event) => setTexto(event.target.value)} required /></div>
              <button className="btn btn-primario" type="submit">Publicar</button>
            </form>
          </div>
        )}
        {errorAccion && <ErrorCaja mensaje={errorAccion} />}
      </section>
      <Confirmar
        abierto={Boolean(foroAArchivar)}
        titulo="Archivar foro"
        texto={foroAArchivar && `El foro "${foroAArchivar.titulo}" dejará de mostrarse como activo.`}
        alConfirmar={archivarForo}
        alCancelar={() => setForoAArchivar(null)}
      />
    </>
  )
}
