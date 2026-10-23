import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { api } from '../../api/index.js'
import { useAviso } from '../../context/ToastContext.jsx'
import { useAuth } from '../../context/AuthContext.jsx'
import { tienePermisoAdmin } from '../../utils/permisos.js'
import useAsync from '../../hooks/useAsync.js'
import { Cargando, Confirmar, Encabezado, ErrorCaja, Vacio } from '../../components/ui.jsx'

// Administración: matricular y quitar estudiantes de un curso.
export default function CursoAdmin() {
  const id = Number(useParams().id)
  const { usuario } = useAuth()
  const puedeMatricular = tienePermisoAdmin(usuario, 'cursos.matricular')
  const avisar = useAviso()
  const { datos: curso, cargando, error, recargar } = useAsync(() => api.cursos.obtener(id), [id])
  const { datos: usuarios } = useAsync(() => api.usuarios.listar(), [])
  const [elegido, setElegido] = useState('')
  const [quitar, setQuitar] = useState(null)
  const [ocupado, setOcupado] = useState(false)

  if (cargando && !curso) return <Cargando />
  if (error) return <ErrorCaja mensaje={error} alReintentar={recargar} />

  const matriculados = new Set(curso.estudiantes.map((e) => e.id))
  const disponibles = (usuarios ?? []).filter((u) => u.rol === 'estudiante' && u.activo && !matriculados.has(u.id))

  async function matricular(e) {
    e.preventDefault()
    try {
      await api.cursos.matricular(id, Number(elegido))
      setElegido('')
      avisar('Estudiante matriculado.')
      recargar()
    } catch (err) { avisar(err.message, 'error') }
  }

  async function confirmarQuitar() {
    setOcupado(true)
    try {
      await api.cursos.desmatricular(id, quitar.id)
      avisar('Matrícula eliminada.')
      setQuitar(null)
      recargar()
    } catch (err) { avisar(err.message, 'error') } finally { setOcupado(false) }
  }

  return (
    <>
      <p><Link to="/app/cursos">← Cursos</Link></p>
      <Encabezado titulo={curso.nombre} subtitulo={`${curso.codigo} · ${curso.periodo} · Docente: ${curso.docente}`} />
      {curso.descripcion && <p>{curso.descripcion}</p>}

      {puedeMatricular && <section className="tarjeta">
        <h2>Matricular estudiante</h2>
        <form className="fila-form" onSubmit={matricular}>
          <select aria-label="Estudiante" value={elegido} onChange={(e) => setElegido(e.target.value)} required>
            <option value="" disabled>Elige un estudiante…</option>
            {disponibles.map((u) => <option key={u.id} value={u.id}>{u.nombre}</option>)}
          </select>
          <button className="btn btn-primario" type="submit" disabled={!elegido}>Matricular</button>
        </form>
      </section>}

      <section className="tarjeta">
        <h2>Estudiantes matriculados ({curso.estudiantes.length})</h2>
        {curso.estudiantes.length === 0 ? (
          <Vacio icono="bi-person-x" titulo="Sin estudiantes" texto="Matricula al primero con el formulario de arriba." />
        ) : (
          <ul className="lista">
            {curso.estudiantes.map((e) => (
              <li key={e.id}>
                <div><strong>{e.nombre}</strong><div className="suave">{e.correo}</div></div>
                {puedeMatricular && <button type="button" className="btn btn-peligro-claro btn-chico" onClick={() => setQuitar(e)}>Quitar</button>}
              </li>
            ))}
          </ul>
        )}
      </section>

      {puedeMatricular && <Confirmar
        abierto={!!quitar}
        titulo="Quitar matrícula"
        textoBoton="Quitar"
        texto={quitar && `Se quitará a ${quitar.nombre} del curso y se borrarán sus notas, asistencia y entregas en él.`}
        alConfirmar={confirmarQuitar}
        alCancelar={() => setQuitar(null)}
        ocupado={ocupado}
      />}
    </>
  )
}
