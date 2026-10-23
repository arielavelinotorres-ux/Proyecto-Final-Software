import { api } from '../../api/index.js'
import useAsync from '../../hooks/useAsync.js'
import { Cargando, Encabezado, ErrorCaja, Vacio } from '../../components/ui.jsx'
import { fechaHora } from '../../utils/formato.js'

export default function Auditoria() {
  const { datos, cargando, error, recargar } = useAsync(() => api.admin.auditoria(200), [])
  if (cargando) return <Cargando />
  if (error) return <ErrorCaja mensaje={error} alReintentar={recargar} />

  return (
    <>
      <Encabezado titulo="Auditoría" subtitulo="Registro de cambios administrativos y académicos." />
      {!datos?.length && <Vacio icono="bi-shield-check" titulo="Sin actividad registrada" texto="Las acciones recientes aparecerán aquí." />}
      {datos?.length > 0 && (
        <div className="tabla-caja">
          <table className="tabla">
            <thead><tr><th>Fecha</th><th>Usuario</th><th>Acción</th><th>Elemento</th><th>Detalle</th></tr></thead>
            <tbody>{datos.map((evento) => (
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
    </>
  )
}