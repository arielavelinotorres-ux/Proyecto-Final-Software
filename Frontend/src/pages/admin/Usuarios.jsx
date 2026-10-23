import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../../api/index.js'
import { useAuth } from '../../context/AuthContext.jsx'
import { useAviso } from '../../context/ToastContext.jsx'
import useAsync from '../../hooks/useAsync.js'
import { Badge, Cargando, Confirmar, Encabezado, ErrorCaja, Modal, Vacio } from '../../components/ui.jsx'
import { tienePermisoAdmin } from '../../utils/permisos.js'

const ROLES = { admin: 'Administración', docente: 'Docente', estudiante: 'Estudiante' }
const TIPOS_ADMIN = { general: 'General', personalizado: 'Personalizado', usuarios: 'Usuarios', cursos: 'Cursos', reportes: 'Reportes' }
const GRUPOS_PERMISOS = [
  { titulo: 'Usuarios', permisos: [['usuarios.crear', 'Crear cuentas'], ['usuarios.editar', 'Editar y activar cuentas'], ['usuarios.eliminar', 'Eliminar cuentas'], ['usuarios.restablecer', 'Restablecer contraseñas']] },
  { titulo: 'Cursos', permisos: [['cursos.consultar', 'Consultar cursos'], ['cursos.crear', 'Crear cursos'], ['cursos.editar', 'Editar cursos'], ['cursos.eliminar', 'Eliminar cursos'], ['cursos.matricular', 'Gestionar matrículas']] },
  { titulo: 'Supervisión', permisos: [['reportes.consultar', 'Consultar reportes'], ['auditoria.consultar', 'Consultar auditoría']] },
]

function permisosIniciales(usuario) {
  const permisos = usuario?.permisosAdmin ?? []
  if (permisos.includes('general')) return ['general']
  const especificos = GRUPOS_PERMISOS.flatMap((grupo) => grupo.permisos.map(([permiso]) => permiso))
  const iniciales = permisos.filter((permiso) => especificos.includes(permiso))
  if (iniciales.length) return iniciales
  if (permisos.includes('usuarios')) iniciales.push(...GRUPOS_PERMISOS[0].permisos.map(([permiso]) => permiso))
  if (permisos.includes('cursos')) iniciales.push(...GRUPOS_PERMISOS[1].permisos.map(([permiso]) => permiso))
  if (permisos.includes('reportes')) iniciales.push('reportes.consultar', 'auditoria.consultar')
  return [...new Set(iniciales)]
}

export default function Usuarios({ modoSeguridad = false }) {
  const { usuario: yo } = useAuth()
  const avisar = useAviso()
  const { datos, cargando, error, recargar } = useAsync(() => api.usuarios.listar(), [])
  const [buscar, setBuscar] = useState('')
  const [rol, setRol] = useState('todos')
  const [editando, setEditando] = useState(null)
  const [restableciendo, setRestableciendo] = useState(null)
  const [aEliminar, setAEliminar] = useState(null)
  const [ocupado, setOcupado] = useState(false)

  const filtrados = useMemo(() => {
    const q = buscar.trim().toLowerCase()
    return (datos ?? []).filter((u) => (!modoSeguridad || u.bloqueado || u.solicitudRestablecimiento) && (rol === 'todos' || u.rol === rol) && (!q || u.nombre.toLowerCase().includes(q) || u.correo.includes(q)))
  }, [datos, buscar, rol, modoSeguridad])

  async function alternarActivo(u) {
    try {
      await api.usuarios.actualizar(u.id, { ...u, activo: !u.activo })
      avisar(u.activo ? 'Cuenta desactivada.' : 'Cuenta activada.')
      recargar()
    } catch (err) { avisar(err.message, 'error') }
  }

  async function eliminar() {
    setOcupado(true)
    try {
      await api.usuarios.eliminar(aEliminar.id)
      avisar('Usuario eliminado.')
      setAEliminar(null)
      recargar()
    } catch (err) { avisar(err.message, 'error'); setAEliminar(null) } finally { setOcupado(false) }
  }

  return (
    <>
      <Encabezado titulo={modoSeguridad ? 'Seguridad de cuentas' : 'Usuarios'} subtitulo={modoSeguridad ? 'Cuentas bloqueadas y solicitudes de restablecimiento.' : 'Cuentas de administración, docentes y estudiantes.'}>
        {modoSeguridad
          ? <Link className="btn btn-claro" to="/app/usuarios"><i className="bi bi-people" /> Todos los usuarios</Link>
          : tienePermisoAdmin(yo, 'usuarios.crear')
            ? <button type="button" className="btn btn-primario" onClick={() => setEditando({})}><i className="bi bi-person-plus" /> Nuevo usuario</button>
            : null}
      </Encabezado>

      <div className="filtros">
        <input type="search" placeholder="Buscar por nombre o correo…" aria-label="Buscar usuarios" value={buscar} onChange={(e) => setBuscar(e.target.value)} />
        <select aria-label="Filtrar por rol" value={rol} onChange={(e) => setRol(e.target.value)}>
          <option value="todos">Todos los roles</option>
          {Object.entries(ROLES).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
        </select>
      </div>

      {cargando && <Cargando />}
      {error && <ErrorCaja mensaje={error} alReintentar={recargar} />}
      {datos && filtrados.length === 0 && <Vacio icono={modoSeguridad ? 'bi-shield-check' : 'bi-search'} titulo={modoSeguridad ? 'Sin alertas de acceso' : 'Sin resultados'} texto={modoSeguridad ? 'No hay cuentas bloqueadas ni solicitudes pendientes.' : 'Prueba con otra búsqueda.'} />}

      {filtrados.length > 0 && (
        <div className="tabla-caja">
          <table className="tabla">
            <thead><tr><th>Nombre</th><th>Correo</th><th>Rol</th><th>Estado</th><th>Acceso</th><th><span className="sr-only">Acciones</span></th></tr></thead>
            <tbody>
              {filtrados.map((u) => (
                <tr key={u.id}>
                  <td data-etiqueta="Nombre"><strong>{u.nombre}</strong>{u.id === yo.id && <small className="suave"> (tú)</small>}</td>
                  <td data-etiqueta="Correo">{u.correo}</td>
                  <td data-etiqueta="Rol">{u.rol === 'admin' ? `Admin · ${TIPOS_ADMIN[u.tipoAdmin] ?? 'General'}` : ROLES[u.rol]}</td>
                  <td data-etiqueta="Estado">{u.activo ? <Badge tono="bajo" icono="bi-check-circle-fill">Activa</Badge> : <Badge tono="neutro" icono="bi-slash-circle">Inactiva</Badge>}</td>
                  <td data-etiqueta="Acceso">{u.bloqueado
                    ? <Badge tono="alto" icono="bi-lock-fill">Bloqueada</Badge>
                    : u.solicitudRestablecimiento
                      ? <Badge tono="moderado" icono="bi-key-fill">Solicitó restablecimiento</Badge>
                      : <span className="suave">Normal</span>}</td>
                  <td className="celda-acciones">
                    {(u.rol !== 'admin' || tienePermisoAdmin(yo, 'general')) && tienePermisoAdmin(yo, 'usuarios.editar') && <button type="button" className="btn btn-claro btn-chico" onClick={() => setEditando(u)}>Editar</button>}
                    {u.rol !== 'admin' && tienePermisoAdmin(yo, 'usuarios.restablecer') && <button type="button" className="btn btn-claro btn-chico" onClick={() => setRestableciendo(u)}>Restablecer clave</button>}
                    {u.id !== yo.id && (u.rol !== 'admin' || yo.permisosAdmin?.includes('general')) && (
                      <>
                        {tienePermisoAdmin(yo, 'usuarios.editar') && <button type="button" className="btn btn-claro btn-chico" onClick={() => alternarActivo(u)}>{u.activo ? 'Desactivar' : 'Activar'}</button>}
                        {tienePermisoAdmin(yo, 'usuarios.eliminar') && <button type="button" className="btn btn-peligro-claro btn-chico" onClick={() => setAEliminar(u)} aria-label={`Eliminar a ${u.nombre}`}><i className="bi bi-trash" /></button>}
                      </>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <FormularioUsuario usuario={editando} yo={yo} alCerrar={() => setEditando(null)} alGuardar={() => { setEditando(null); recargar() }} />
      <FormularioRestablecer usuario={restableciendo} alCerrar={() => setRestableciendo(null)} alGuardar={() => { setRestableciendo(null); recargar() }} />
      <Confirmar
        abierto={!!aEliminar}
        titulo="Eliminar usuario"
        texto={aEliminar && `Se eliminará a ${aEliminar.nombre} junto con sus notas, asistencia y entregas.`}
        alConfirmar={eliminar}
        alCancelar={() => setAEliminar(null)}
        ocupado={ocupado}
      />
    </>
  )
}

function FormularioRestablecer({ usuario, alCerrar, alGuardar }) {
  const avisar = useAviso()
  const [nueva, setNueva] = useState('')
  const [error, setError] = useState(null)
  const [enviando, setEnviando] = useState(false)

  useEffect(() => {
    setNueva('')
    setError(null)
  }, [usuario?.id])

  async function enviar(e) {
    e.preventDefault()
    if (form.rol === 'admin' && !form.permisosAdmin?.length) {
      setError('Selecciona al menos una capacidad para el administrador.')
      return
    }
    setEnviando(true)
    setError(null)
    try {
      await api.usuarios.restablecerClave(usuario.id, nueva)
      avisar('Contraseña restablecida y cuenta desbloqueada.')
      setNueva('')
      alGuardar()
    } catch (err) { setError(err.message) } finally { setEnviando(false) }
  }

  return (
    <Modal abierto={usuario !== null} titulo="Restablecer contraseña" alCerrar={alCerrar}>
      <form onSubmit={enviar} className="form-modal">
        <p>Se desbloqueará la cuenta de <strong>{usuario?.nombre}</strong> y se borrará la solicitud pendiente.</p>
        <div className="campo">
          <label htmlFor="clave-restablecida">Nueva contraseña</label>
          <input id="clave-restablecida" type="password" minLength={8} autoComplete="new-password" value={nueva} onChange={(e) => setNueva(e.target.value)} required />
          <small className="ayuda">Mínimo 8 caracteres.</small>
        </div>
        {error && <p className="msg msg-error" role="alert">{error}</p>}
        <div className="acciones">
          <button type="button" className="btn btn-claro" onClick={alCerrar}>Cancelar</button>
          <button type="submit" className="btn btn-primario" disabled={enviando}>{enviando ? 'Restableciendo…' : 'Restablecer y desbloquear'}</button>
        </div>
      </form>
    </Modal>
  )
}

function FormularioUsuario({ usuario, yo, alCerrar, alGuardar }) {
  const avisar = useAviso()
  const [form, setForm] = useState({})
  const [error, setError] = useState(null)
  const [enviando, setEnviando] = useState(false)
  const [ultimo, setUltimo] = useState(null)
  const nuevo = usuario && !usuario.id

  if (usuario !== ultimo) {
    setUltimo(usuario)
    if (usuario) {
      setForm({ nombre: usuario.nombre ?? '', correo: usuario.correo ?? '', rol: usuario.rol ?? 'estudiante', tipoAdmin: usuario.tipoAdmin ?? 'personalizado', permisosAdmin: permisosIniciales(usuario), clave: '', activo: usuario.activo ?? true })
      setError(null)
    }
  }
  const cambiar = (k) => (e) => setForm({ ...form, [k]: e.target.value })
  const cambiarPermiso = (permiso, activo) => setForm((actual) => ({
    ...actual,
    permisosAdmin: activo
      ? [...new Set([...(actual.permisosAdmin ?? []).filter((p) => p !== 'general'), permiso])]
      : (actual.permisosAdmin ?? []).filter((p) => p !== permiso),
  }))

  async function enviar(e) {
    e.preventDefault()
    setEnviando(true)
    setError(null)
    try {
      const datos = { ...form, tipoAdmin: form.rol === 'admin' && form.permisosAdmin?.includes('general') ? 'general' : 'personalizado' }
      if (nuevo) await api.usuarios.crear(datos)
      else await api.usuarios.actualizar(usuario.id, datos)
      avisar(nuevo ? 'Usuario creado.' : 'Usuario actualizado.')
      alGuardar()
    } catch (err) { setError(err.message) } finally { setEnviando(false) }
  }

  return (
    <Modal abierto={usuario !== null} titulo={nuevo ? 'Nuevo usuario' : 'Editar usuario'} alCerrar={alCerrar}>
      <form onSubmit={enviar} className="form-modal">
        <div className="campo"><label htmlFor="u-nombre">Nombre</label><input id="u-nombre" value={form.nombre ?? ''} onChange={cambiar('nombre')} required /></div>
        <div className="campo"><label htmlFor="u-correo">Correo</label><input id="u-correo" type="email" value={form.correo ?? ''} onChange={cambiar('correo')} required /></div>
        <div className="campo">
          <label htmlFor="u-rol">Rol</label>
          <select id="u-rol" value={form.rol ?? 'estudiante'} onChange={cambiar('rol')} disabled={usuario?.id === yo.id}>
            {Object.entries(ROLES).filter(([k]) => k !== 'admin' || yo.permisosAdmin?.includes('general')).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
          </select>
        </div>
        {form.rol === 'admin' && yo.permisosAdmin?.includes('general') && <fieldset className="sub-form">
          <legend>Permisos del administrador</legend>
          <label className="interruptor">
            <input type="checkbox" checked={form.permisosAdmin?.includes('general') ?? false} onChange={(e) => setForm({ ...form, permisosAdmin: e.target.checked ? ['general'] : [] })} disabled={usuario?.id === yo.id} />
            Administrador general, acceso total
          </label>
          {!form.permisosAdmin?.includes('general') && GRUPOS_PERMISOS.map((grupo) => (
            <fieldset className="sub-form" key={grupo.titulo}>
              <legend>{grupo.titulo}</legend>
              {grupo.permisos.map(([permiso, etiqueta]) => (
                <label className="interruptor" key={permiso}>
                  <input type="checkbox" checked={form.permisosAdmin?.includes(permiso) ?? false} onChange={(e) => cambiarPermiso(permiso, e.target.checked)} disabled={usuario?.id === yo.id} />
                  {etiqueta}
                </label>
              ))}
            </fieldset>
          ))}
          <small className="ayuda">Una cuenta puede recibir varias capacidades; cada acción se valida también en el servidor.</small>
        </fieldset>}
        {nuevo && (
          <div className="campo">
            <label htmlFor="u-clave">Contraseña inicial</label>
            <input id="u-clave" type="password" minLength={8} autoComplete="new-password" value={form.clave ?? ''} onChange={cambiar('clave')} required />
            <small className="ayuda">Mínimo 8 caracteres.</small>
          </div>
        )}
        {error && <p className="msg msg-error" role="alert">{error}</p>}
        <div className="acciones">
          <button type="button" className="btn btn-claro" onClick={alCerrar}>Cancelar</button>
          <button type="submit" className="btn btn-primario" disabled={enviando}>{enviando ? 'Guardando…' : 'Guardar'}</button>
        </div>
      </form>
    </Modal>
  )
}
