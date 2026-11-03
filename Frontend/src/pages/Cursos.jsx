import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/index.js'
import { useAuth } from '../context/AuthContext.jsx'
import { useAviso } from '../context/ToastContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { Cargando, Confirmar, Encabezado, ErrorCaja, Modal, Vacio } from '../components/ui.jsx'

const CURSO_VACIO = {
  codigo: '',
  nombre: '',
  descripcion: '',
  docenteId: '',
  subcategoriaId: '',
  estado: 'Borrador',
}

export default function Cursos() {
  const { usuario } = useAuth()
  const avisar = useAviso()
  const { datos: cursos, cargando, error, recargar } = useAsync(() => api.cursos.listar(), [])
  const [editando, setEditando] = useState(null)
  const [eliminando, setEliminando] = useState(null)
  const [ocupado, setOcupado] = useState(false)
  const esAdmin = usuario.rol === 'admin'

  async function eliminar() {
    setOcupado(true)
    try {
      await api.cursos.eliminar(eliminando.id)
      setEliminando(null)
      avisar('Curso eliminado.')
      recargar()
    } catch (err) {
      avisar(err.message, 'error')
    } finally {
      setOcupado(false)
    }
  }

  return (
    <>
      <Encabezado titulo={esAdmin ? 'Cursos' : 'Mis cursos'} subtitulo="Datos del catálogo académico de la base de datos.">
        {esAdmin && <button className="btn btn-primario" type="button" onClick={() => setEditando(CURSO_VACIO)}>Nuevo curso</button>}
      </Encabezado>
      {cargando && <Cargando />}
      {error && <ErrorCaja mensaje={error} alReintentar={recargar} />}
      {cursos?.length === 0 && <Vacio icono="bi-journal-x" titulo="Sin cursos" texto="No se encontraron cursos asociados a tu cuenta." />}
      <div className="rejilla-cursos">
        {cursos?.map((curso) => (
          <article key={curso.id} className="curso-tarjeta">
            <p className="demo-etiqueta">{curso.codigo} · {curso.estado}</p>
            <h2><Link to={`/app/cursos/${curso.id}`}>{curso.nombre}</Link></h2>
            <p className="suave">{curso.categoria} / {curso.subcategoria}</p>
            <p className="suave">Docente: {curso.docente} · {curso.estudiantes} estudiantes</p>
            {curso.descripcion && <p>{curso.descripcion}</p>}
            <div className="acciones-linea">
              <Link className="btn btn-claro" to={`/app/cursos/${curso.id}`}>Abrir</Link>
              {esAdmin && <>
                <button className="btn btn-claro" type="button" onClick={() => setEditando(curso)}>Editar</button>
                <button className="btn btn-peligro-claro" type="button" onClick={() => setEliminando(curso)}>Eliminar</button>
              </>}
            </div>
          </article>
        ))}
      </div>
      {esAdmin && (
        <FormularioCurso
          curso={editando}
          alCerrar={() => setEditando(null)}
          alGuardar={() => { setEditando(null); recargar() }}
          avisar={avisar}
        />
      )}
      <Confirmar
        abierto={Boolean(eliminando)}
        titulo="Eliminar curso"
        texto={eliminando && `Se eliminará "${eliminando.nombre}". SQL Server impedirá la operación si aún tiene contenido o matrículas asociadas.`}
        alConfirmar={eliminar}
        alCancelar={() => setEliminando(null)}
        ocupado={ocupado}
      />
    </>
  )
}

function FormularioCurso({ curso, alCerrar, alGuardar, avisar }) {
  const [form, setForm] = useState(CURSO_VACIO)
  const [enviando, setEnviando] = useState(false)
  const [error, setError] = useState(null)
  const { datos: subcategorias, error: errorSubcategorias, recargar: recargarSubcategorias } = useAsync(() => api.catalogo.subcategorias(), [])
  const { datos: docentes, error: errorDocentes, recargar: recargarDocentes } = useAsync(() => api.usuarios.listar('docente'), [])

  useEffect(() => {
    if (!curso) return
    setForm({
      codigo: curso.codigo ?? '',
      nombre: curso.nombre ?? '',
      descripcion: curso.descripcion ?? '',
      docenteId: curso.docenteId ?? '',
      subcategoriaId: curso.subcategoriaId ?? '',
      estado: curso.estado ?? 'Borrador',
    })
    setError(null)
  }, [curso])

  function cambiar(event) {
    setForm((actual) => ({ ...actual, [event.target.name]: event.target.value }))
  }

  async function enviar(event) {
    event.preventDefault()
    setEnviando(true)
    setError(null)
    const data = {
      ...form,
      docenteId: Number(form.docenteId),
      subcategoriaId: Number(form.subcategoriaId),
    }
    try {
      if (curso.id) await api.cursos.actualizar(curso.id, data)
      else await api.cursos.crear(data)
      avisar(curso.id ? 'Curso actualizado.' : 'Curso creado.')
      alGuardar()
    } catch (err) {
      setError(err.message)
    } finally {
      setEnviando(false)
    }
  }

  return (
    <Modal abierto={Boolean(curso)} titulo={curso?.id ? 'Editar curso' : 'Nuevo curso'} alCerrar={alCerrar}>
      <form className="form-modal" onSubmit={enviar}>
        {errorSubcategorias && <ErrorCaja mensaje={errorSubcategorias} alReintentar={recargarSubcategorias} />}
        {errorDocentes && <ErrorCaja mensaje={errorDocentes} alReintentar={recargarDocentes} />}
        <div className="campo"><label htmlFor="curso-codigo">Código</label><input id="curso-codigo" name="codigo" maxLength="20" value={form.codigo} onChange={cambiar} required /></div>
        <div className="campo"><label htmlFor="curso-nombre">Nombre</label><input id="curso-nombre" name="nombre" maxLength="150" value={form.nombre} onChange={cambiar} required /></div>
        <div className="campo">
          <label htmlFor="curso-docente">Docente</label>
          <select id="curso-docente" name="docenteId" value={form.docenteId} onChange={cambiar} required>
            <option value="">Selecciona un docente</option>
            {docentes?.map((docente) => <option key={docente.id} value={docente.id}>{docente.nombre}</option>)}
          </select>
        </div>
        <div className="campo">
          <label htmlFor="curso-subcategoria">Subcategoría</label>
          <select id="curso-subcategoria" name="subcategoriaId" value={form.subcategoriaId} onChange={cambiar} required>
            <option value="">Selecciona una subcategoría</option>
            {subcategorias?.filter((item) => item.activo).map((item) => (
              <option key={item.id} value={item.id}>{item.categoria} / {item.nombre}</option>
            ))}
          </select>
        </div>
        <div className="campo">
          <label htmlFor="curso-estado">Estado</label>
          <select id="curso-estado" name="estado" value={form.estado} onChange={cambiar}>
            <option value="Borrador">Borrador</option>
            <option value="Publicado">Publicado</option>
            <option value="Archivado">Archivado</option>
          </select>
        </div>
        <div className="campo"><label htmlFor="curso-descripcion">Descripción</label><textarea id="curso-descripcion" name="descripcion" value={form.descripcion} onChange={cambiar} rows={3} /></div>
        {error && <p className="msg msg-error" role="alert">{error}</p>}
        <div className="acciones">
          <button type="button" className="btn btn-claro" onClick={alCerrar}>Cancelar</button>
          <button type="submit" className="btn btn-primario" disabled={enviando}>{enviando ? 'Guardando…' : 'Guardar'}</button>
        </div>
      </form>
    </Modal>
  )
}
