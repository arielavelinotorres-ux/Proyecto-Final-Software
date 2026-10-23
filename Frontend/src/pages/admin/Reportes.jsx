import { api } from '../../api/index.js'
import { useAuth } from '../../context/AuthContext.jsx'
import { tienePermisoAdmin } from '../../utils/permisos.js'
import useAsync from '../../hooks/useAsync.js'
import { Cargando, Encabezado, ErrorCaja, StatCard, Vacio } from '../../components/ui.jsx'
import { fechaHora } from '../../utils/formato.js'

export default function Reportes() {
  const { usuario } = useAuth()
  const puedeAuditar = tienePermisoAdmin(usuario, 'auditoria.consultar')
  const { datos, cargando, error, recargar } = useAsync(async () => {
    const [usuarios, cursos, auditoria] = await Promise.all([
      api.usuarios.listar(), api.cursos.listar(), puedeAuditar ? api.admin.auditoria(200) : Promise.resolve([]),
    ])
    return { usuarios, cursos, auditoria }
  }, [puedeAuditar])

  if (cargando) return <Cargando />
  if (error) return <ErrorCaja mensaje={error} alReintentar={recargar} />

  return (
    <>
      <Encabezado titulo="Reportes y auditoría" subtitulo="Exporta datos institucionales y consulta cambios recientes." />
      <dl className="stats">
        <StatCard etiqueta="Usuarios" valor={datos.usuarios.length} icono="bi-people" />
        <StatCard etiqueta="Cursos" valor={datos.cursos.length} icono="bi-journal-bookmark" />
        {puedeAuditar && <StatCard etiqueta="Eventos recientes" valor={datos.auditoria.length} icono="bi-shield-check" />}
      </dl>
      <section className="tarjeta reportes-exportar">
        <h2>Exportaciones</h2>
        <p className="suave">Los archivos CSV incluyen los registros actuales, sin contraseñas.</p>
        <div className="acciones-linea">
          <button type="button" className="btn btn-claro" onClick={() => descargarCsv('usuarios-homeclass.csv', ['id', 'nombre', 'correo', 'rol', 'activo'], datos.usuarios)}><i className="bi bi-download" /> Usuarios</button>
          <button type="button" className="btn btn-claro" onClick={() => descargarCsv('cursos-homeclass.csv', ['id', 'codigo', 'nombre', 'periodo', 'docente', 'estudiantes', 'promedio', 'asistencia'], datos.cursos)}><i className="bi bi-download" /> Cursos</button>
          {puedeAuditar && <button type="button" className="btn btn-claro" onClick={() => descargarCsv('auditoria-homeclass.csv', ['fecha', 'usuario', 'accion', 'entidad', 'entidadId', 'detalle'], datos.auditoria)}><i className="bi bi-download" /> Auditoría</button>}
        </div>
      </section>
      {puedeAuditar && <section className="reportes-auditoria">
        <header className="seccion-cabecera"><div><h2>Actividad reciente</h2><p className="suave">Últimos {datos.auditoria.length} eventos registrados.</p></div></header>
        {!datos.auditoria.length && <Vacio icono="bi-shield-check" titulo="Sin actividad registrada" texto="Las acciones administrativas y académicas aparecerán aquí." />}
        {datos.auditoria.length > 0 && (
          <div className="tabla-caja">
            <table className="tabla">
              <thead><tr><th>Fecha</th><th>Usuario</th><th>Acción</th><th>Elemento</th><th>Detalle</th></tr></thead>
              <tbody>{datos.auditoria.map((evento) => (
                <tr key={evento.id}>
                  <td data-etiqueta="Fecha">{fechaHora(evento.fecha)}</td>
                  <td data-etiqueta="Usuario">{evento.usuario}</td>
                  <td data-etiqueta="Acción">{evento.accion}</td>
                  <td data-etiqueta="Elemento">{evento.entidad}{evento.entidadId ? ` #${evento.entidadId}` : ''}</td>
                  <td data-etiqueta="Detalle">{evento.detalle || '—'}</td>
                </tr>
              ))}</tbody>
            </table>
          </div>
        )}
      </section>}
    </>
  )
}

function descargarCsv(nombre, columnas, filas) {
  const escapar = (valor) => {
    let texto = valor === null || valor === undefined ? '' : String(valor)
    if (/^[=+@\-]/.test(texto)) texto = `'${texto}`
    return `"${texto.replaceAll('"', '""')}"`
  }
  const csv = `\uFEFF${[columnas, ...filas.map((fila) => columnas.map((columna) => fila[columna]))].map((fila) => fila.map(escapar).join(',')).join('\r\n')}`
  const url = URL.createObjectURL(new Blob([csv], { type: 'text/csv;charset=utf-8' }))
  const enlace = document.createElement('a')
  enlace.href = url
  enlace.download = nombre
  document.body.appendChild(enlace)
  enlace.click()
  enlace.remove()
  window.setTimeout(() => URL.revokeObjectURL(url), 1000)
}