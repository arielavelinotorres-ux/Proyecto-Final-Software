import { useState } from 'react'
import { api } from '../api/index.js'
import { useAviso } from '../context/ToastContext.jsx'
import useAsync from '../hooks/useAsync.js'
import { Confirmar, Encabezado, ErrorCaja, Vacio } from '../components/ui.jsx'

const CATEGORIA_NUEVA = { nombre: '', descripcion: '', activo: true }
const SUBCATEGORIA_NUEVA = { categoriaId: '', nombre: '', descripcion: '', activo: true }

export default function Categorias() {
  const avisar = useAviso()
  const { datos: categorias, error: errorCategorias, recargar: recargarCategorias } = useAsync(() => api.catalogo.categorias(), [])
  const { datos: subcategorias, error: errorSubcategorias, recargar: recargarSubcategorias } = useAsync(() => api.catalogo.subcategorias(), [])
  const [categoria, setCategoria] = useState(CATEGORIA_NUEVA)
  const [subcategoria, setSubcategoria] = useState(SUBCATEGORIA_NUEVA)
  const [confirmacion, setConfirmacion] = useState(null)
  const [errorAccion, setErrorAccion] = useState(null)

  function recargar() {
    recargarCategorias()
    recargarSubcategorias()
  }

  async function guardarCategoria(event) {
    event.preventDefault()
    setErrorAccion(null)
    try {
      if (categoria.id) await api.catalogo.actualizarCategoria(categoria.id, categoria)
      else await api.catalogo.crearCategoria(categoria)
      setCategoria(CATEGORIA_NUEVA)
      avisar(categoria.id ? 'Categoría actualizada.' : 'Categoría creada.')
      recargar()
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function guardarSubcategoria(event) {
    event.preventDefault()
    setErrorAccion(null)
    const data = { ...subcategoria, categoriaId: Number(subcategoria.categoriaId) }
    try {
      if (subcategoria.id) await api.catalogo.actualizarSubcategoria(subcategoria.id, data)
      else await api.catalogo.crearSubcategoria(data)
      setSubcategoria(SUBCATEGORIA_NUEVA)
      avisar(subcategoria.id ? 'Subcategoría actualizada.' : 'Subcategoría creada.')
      recargar()
    } catch (err) {
      setErrorAccion(err.message)
    }
  }

  async function eliminar() {
    if (!confirmacion) return
    try {
      if (confirmacion.tipo === 'categoria') await api.catalogo.eliminarCategoria(confirmacion.item.id)
      else await api.catalogo.eliminarSubcategoria(confirmacion.item.id)
      avisar('Registro eliminado.')
      setConfirmacion(null)
      recargar()
    } catch (err) {
      setErrorAccion(err.message)
      setConfirmacion(null)
    }
  }

  return (
    <>
      <Encabezado titulo="Categorías académicas" subtitulo="Administración del catálogo usando los procedimientos almacenados de Academico." />
      {errorCategorias && <ErrorCaja mensaje={errorCategorias} alReintentar={recargarCategorias} />}
      {errorSubcategorias && <ErrorCaja mensaje={errorSubcategorias} alReintentar={recargarSubcategorias} />}
      {errorAccion && <ErrorCaja mensaje={errorAccion} />}
      <div className="rejilla">
        <form className="tarjeta form" onSubmit={guardarCategoria}>
          <h2>{categoria.id ? 'Editar categoría' : 'Nueva categoría'}</h2>
          <div className="campo"><label htmlFor="categoria-nombre">Nombre</label><input id="categoria-nombre" maxLength="100" value={categoria.nombre} onChange={(event) => setCategoria({ ...categoria, nombre: event.target.value })} required /></div>
          <div className="campo"><label htmlFor="categoria-descripcion">Descripción</label><input id="categoria-descripcion" maxLength="255" value={categoria.descripcion ?? ''} onChange={(event) => setCategoria({ ...categoria, descripcion: event.target.value })} /></div>
          <label><input type="checkbox" checked={categoria.activo} onChange={(event) => setCategoria({ ...categoria, activo: event.target.checked })} /> Activa</label>
          <div className="acciones">
            {categoria.id && <button className="btn btn-claro" type="button" onClick={() => setCategoria(CATEGORIA_NUEVA)}>Cancelar</button>}
            <button className="btn btn-primario" type="submit">Guardar categoría</button>
          </div>
        </form>

        <form className="tarjeta form" onSubmit={guardarSubcategoria}>
          <h2>{subcategoria.id ? 'Editar subcategoría' : 'Nueva subcategoría'}</h2>
          <div className="campo">
            <label htmlFor="subcategoria-categoria">Categoría</label>
            <select id="subcategoria-categoria" value={subcategoria.categoriaId} onChange={(event) => setSubcategoria({ ...subcategoria, categoriaId: event.target.value })} required>
              <option value="">Selecciona una categoría</option>
              {categorias?.filter((item) => item.activo).map((item) => <option key={item.id} value={item.id}>{item.nombre}</option>)}
            </select>
          </div>
          <div className="campo"><label htmlFor="subcategoria-nombre">Nombre</label><input id="subcategoria-nombre" maxLength="100" value={subcategoria.nombre} onChange={(event) => setSubcategoria({ ...subcategoria, nombre: event.target.value })} required /></div>
          <div className="campo"><label htmlFor="subcategoria-descripcion">Descripción</label><input id="subcategoria-descripcion" maxLength="255" value={subcategoria.descripcion ?? ''} onChange={(event) => setSubcategoria({ ...subcategoria, descripcion: event.target.value })} /></div>
          <label><input type="checkbox" checked={subcategoria.activo} onChange={(event) => setSubcategoria({ ...subcategoria, activo: event.target.checked })} /> Activa</label>
          <div className="acciones">
            {subcategoria.id && <button className="btn btn-claro" type="button" onClick={() => setSubcategoria(SUBCATEGORIA_NUEVA)}>Cancelar</button>}
            <button className="btn btn-primario" type="submit">Guardar subcategoría</button>
          </div>
        </form>
      </div>

      <section className="tarjeta">
        <h2>Categorías</h2>
        {!categorias?.length && <Vacio titulo="Sin categorías" texto="Crea una categoría para organizar los cursos." />}
        <ul className="lista">{categorias?.map((item) => (
          <li key={item.id}>
            <div><strong>{item.nombre}</strong><p className="suave">{item.descripcion} · {item.activo ? 'Activa' : 'Inactiva'}</p></div>
            <div className="acciones-linea">
              <button className="btn btn-claro btn-chico" type="button" onClick={() => setCategoria(item)}>Editar</button>
              <button className="btn btn-peligro-claro btn-chico" type="button" onClick={() => setConfirmacion({ tipo: 'categoria', item })}>Eliminar</button>
            </div>
          </li>
        ))}</ul>
      </section>
      <section className="tarjeta">
        <h2>Subcategorías</h2>
        {!subcategorias?.length && <Vacio titulo="Sin subcategorías" texto="Crea una subcategoría para poder asignarla a un curso." />}
        <ul className="lista">{subcategorias?.map((item) => (
          <li key={item.id}>
            <div><strong>{item.categoria} / {item.nombre}</strong><p className="suave">{item.descripcion} · {item.activo ? 'Activa' : 'Inactiva'}</p></div>
            <div className="acciones-linea">
              <button className="btn btn-claro btn-chico" type="button" onClick={() => setSubcategoria({ ...item, categoriaId: item.categoria_id })}>Editar</button>
              <button className="btn btn-peligro-claro btn-chico" type="button" onClick={() => setConfirmacion({ tipo: 'subcategoria', item })}>Eliminar</button>
            </div>
          </li>
        ))}</ul>
      </section>
      <Confirmar
        abierto={Boolean(confirmacion)}
        titulo="Eliminar registro"
        texto="La base de datos rechazará la eliminación si el registro tiene cursos o dependencias asociadas."
        alConfirmar={eliminar}
        alCancelar={() => setConfirmacion(null)}
      />
    </>
  )
}
