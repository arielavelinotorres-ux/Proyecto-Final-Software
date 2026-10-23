import { useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import { useAviso } from '../context/ToastContext.jsx'
import { tienePermisoAdmin } from '../utils/permisos.js'
import useAsync from '../hooks/useAsync.js'
import { BadgeRiesgo, Cargando, Confirmar, Encabezado, ErrorCaja, Modal, Vacio } from '../components/ui.jsx'
import { num } from '../utils/formato.js'

// Lista de cursos. Admin: crea/edita/elimina. Docente y estudiante: solo entran a sus cursos.
export default function Cursos() {
  const { usuario } = useAuth()
  const avisar = useAviso()
  const { datos, cargando, error, recargar } = useAsync(() => api.cursos.listar(), [])
  const [editando, setEditando] = useState(null) // null | {} (nuevo) | curso
  const [aEliminar, setAEliminar] = useState(null)
  const [ocupado, setOcupado] = useState(false)
  const esAdmin = usuario.rol === 'admin'
  const puedeCrear = tienePermisoAdmin(usuario, 'cursos.crear')
  const puedeEditar = tienePermisoAdmin(usuario, 'cursos.editar')
  const puedeEliminar = tienePermisoAdmin(usuario, 'cursos.eliminar')

  async function eliminar() {
    setOcupado(true)
    try {
      await api.cursos.eliminar(aEliminar.id)
      avisar('Curso eliminado.')
      setAEliminar(null)
      recargar()
    } catch (err) { avisar(err.message, 'error') } finally { setOcupado(false) }
  }

  return (
    <>
      <Encabezado titulo={esAdmin ? 'Cursos' : 'Mis cursos'} subtitulo={esAdmin ? 'Gestiona los cursos según las capacidades asignadas.' : undefined}>
        {puedeCrear && <button type="button" className="btn btn-primario" onClick={() => setEditando({})}><i className="bi bi-plus-lg" /> Nuevo curso</button>}
      </Encabezado>

      {cargando && <Cargando />}
      {error && <ErrorCaja mensaje={error} alReintentar={recargar} />}
      {datos && datos.length === 0 && (
        <Vacio icono="bi-journal-x" titulo="Sin cursos" texto={esAdmin ? 'Crea el primer curso.' : 'Todavía no tienes cursos asignados.'} />
      )}

      <div className="rejilla-cursos">
        {datos?.map((c) => (
          <article key={c.id} className="curso-tarjeta">
            <p className="demo-etiqueta">{c.codigo} · {c.periodo}</p>
            <h2><Link to={`/app/cursos/${c.id}`}>{c.nombre}</Link></h2>
            <p className="suave">{usuario.rol === 'estudiante' ? `Docente: ${c.docente}` : `${c.estudiantes} estudiantes${esAdmin ? ` · ${c.docente}` : ''}`}</p>
            <dl className="mini-stats">
              <div><dt>Promedio</dt><dd>{num(c.promedio)}</dd></div>
              <div><dt>Asistencia</dt><dd>{num(c.asistencia, '%')}</dd></div>
              {usuario.rol === 'estudiante'
                ? <div><dt>Estado</dt><dd><BadgeRiesgo nivel={c.riesgo} /></dd></div>
                : <div><dt>En riesgo</dt><dd>{c.enRiesgo}</dd></div>}
            </dl>
            <div className="acciones-linea">
              <Link className="btn btn-claro" to={`/app/cursos/${c.id}`}>Abrir</Link>
              {esAdmin && (puedeEditar || puedeEliminar) && (
                <>
                  {puedeEditar && <button type="button" className="btn btn-claro" onClick={() => setEditando(c)}>Editar</button>}
                  {puedeEliminar && <button type="button" className="btn btn-peligro-claro" onClick={() => setAEliminar(c)} aria-label={`Eliminar ${c.nombre}`}><i className="bi bi-trash" /></button>}
                </>
              )}
            </div>
            {!esAdmin && <details className="atajos-curso">
              <summary><i className="bi bi-lightning-charge" /> Accesos rápidos</summary>
              <div className="atajos-curso-enlaces">
                {usuario.rol === 'docente' ? <>
                  <Link to={`/app/cursos/${c.id}?seccion=notas`}><i className="bi bi-journal-check" /> Calificaciones</Link>
                  <Link to={`/app/cursos/${c.id}?seccion=asistencia`}><i className="bi bi-calendar-check" /> Asistencia</Link>
                  <Link to={`/app/cursos/${c.id}?seccion=evaluaciones`}><i className="bi bi-clipboard2-check" /> Evaluaciones</Link>
                  <Link to={`/app/cursos/${c.id}?seccion=entregas`}><i className="bi bi-inbox" /> Entregas</Link>
                  <Link to={`/app/cursos/${c.id}?seccion=materiales`}><i className="bi bi-folder2-open" /> Materiales</Link>
                  <Link to={`/app/cursos/${c.id}?seccion=avisos`}><i className="bi bi-megaphone" /> Avisos</Link>
                </> : <>
                  <Link to={`/app/cursos/${c.id}?seccion=evaluaciones`}><i className="bi bi-journal-check" /> Evaluaciones</Link>
                  <Link to={`/app/cursos/${c.id}?seccion=materiales`}><i className="bi bi-folder2-open" /> Materiales</Link>
                  <Link to={`/app/cursos/${c.id}?seccion=avisos`}><i className="bi bi-megaphone" /> Avisos</Link>
                </>}
              </div>
            </details>}
          </article>
        ))}
      </div>

      {esAdmin && (puedeCrear || puedeEditar) && <FormularioCurso curso={editando} alCerrar={() => setEditando(null)} alGuardar={() => { setEditando(null); recargar() }} />}
      <Confirmar
        abierto={!!aEliminar}
        titulo="Eliminar curso"
        texto={aEliminar && `Se eliminará "${aEliminar.nombre}" con sus evaluaciones, notas, asistencia y archivos. Esta acción no se puede deshacer.`}
        alConfirmar={eliminar}
        alCancelar={() => setAEliminar(null)}
        ocupado={ocupado}
      />
    </>
  )
}

function FormularioCurso({ curso, alCerrar, alGuardar }) {
  const avisar = useAviso()
  const abierto = curso !== null
  const [form, setForm] = useState({})
  const [docentes, setDocentes] = useState([])
  const [error, setError] = useState(null)
  const [enviando, setEnviando] = useState(false)
  const [ultimo, setUltimo] = useState(null)

  // Reinicia el formulario cada vez que se abre (patrón "ajustar estado durante el render").
  if (curso !== ultimo) {
    setUltimo(curso)
    if (curso) {
      setForm({ codigo: curso.codigo ?? '', nombre: curso.nombre ?? '', periodo: curso.periodo ?? '2026-II', descripcion: curso.descripcion ?? '', docenteId: curso.docenteId ?? '' })
      setError(null)
      api.usuarios.listar().then((u) => setDocentes(u.filter((x) => x.rol === 'docente' && x.activo))).catch(() => {})
    }
  }

  async function enviar(e) {
    e.preventDefault()
    setEnviando(true)
    setError(null)
    const datos = { ...form, docenteId: Number(form.docenteId) }
    try {
      if (curso.id) await api.cursos.actualizar(curso.id, datos)
      else await api.cursos.crear(datos)
      avisar(curso.id ? 'Curso actualizado.' : 'Curso creado.')
      alGuardar()
    } catch (err) { setError(err.message) } finally { setEnviando(false) }
  }
  const cambiar = (k) => (e) => setForm({ ...form, [k]: e.target.value })

  return (
    <Modal abierto={abierto} titulo={curso?.id ? 'Editar curso' : 'Nuevo curso'} alCerrar={alCerrar}>
      <form onSubmit={enviar} className="form-modal">
        <div className="campo"><label htmlFor="c-codigo">Código</label><input id="c-codigo" value={form.codigo ?? ''} onChange={cambiar('codigo')} required /></div>
        <div className="campo"><label htmlFor="c-nombre">Nombre</label><input id="c-nombre" value={form.nombre ?? ''} onChange={cambiar('nombre')} required /></div>
        <div className="campo"><label htmlFor="c-periodo">Periodo</label><input id="c-periodo" value={form.periodo ?? ''} onChange={cambiar('periodo')} /></div>
        <div className="campo">
          <label htmlFor="c-docente">Docente</label>
          <select id="c-docente" value={form.docenteId ?? ''} onChange={cambiar('docenteId')} required>
            <option value="" disabled>Elige un docente…</option>
            {docentes.map((d) => <option key={d.id} value={d.id}>{d.nombre}</option>)}
          </select>
        </div>
        <div className="campo"><label htmlFor="c-desc">Descripción</label><textarea id="c-desc" rows={3} value={form.descripcion ?? ''} onChange={cambiar('descripcion')} /></div>
        {error && <p className="msg msg-error" role="alert">{error}</p>}
        <div className="acciones">
          <button type="button" className="btn btn-claro" onClick={alCerrar}>Cancelar</button>
          <button type="submit" className="btn btn-primario" disabled={enviando}>{enviando ? 'Guardando…' : 'Guardar'}</button>
        </div>
      </form>
    </Modal>
  )
}
