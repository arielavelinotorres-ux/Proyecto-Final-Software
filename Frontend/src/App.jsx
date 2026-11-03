import { Navigate, Route, Routes } from 'react-router-dom'
import { useAuth } from './context/AuthContext.jsx'
import PublicLayout from './layouts/PublicLayout.jsx'
import AppLayout from './layouts/AppLayout.jsx'
import RutaProtegida from './components/RutaProtegida.jsx'
import Inicio from './pages/Inicio.jsx'
import { ContactoPage, FuncionesPage, GestionesPage, HistoriaPage, NosotrosPage } from './pages/Publicas.jsx'
import Ingresar from './pages/Ingresar.jsx'
import NoEncontrada from './pages/NoEncontrada.jsx'
import Cursos from './pages/Cursos.jsx'
import CursoDetalle from './pages/CursoDetalle.jsx'
import Categorias from './pages/Categorias.jsx'
import Mensajes from './pages/Mensajes.jsx'
import PanelLms from './pages/PanelLms.jsx'

export default function App() {
  return (
    <Routes>
      <Route element={<PublicLayout />}>
        <Route index element={<Inicio />} />
        <Route path="nosotros" element={<NosotrosPage />} />
        <Route path="historia" element={<HistoriaPage />} />
        <Route path="gestiones" element={<GestionesPage />} />
        <Route path="funciones" element={<FuncionesPage />} />
        <Route path="contacto" element={<ContactoPage />} />
        <Route path="ingresar" element={<Ingresar />} />
      </Route>

      <Route path="app" element={<RutaProtegida />}>
        <Route element={<AppLayout />}>
          <Route index element={<PanelLms />} />
          <Route path="cursos" element={<Cursos />} />
          <Route path="cursos/:id" element={<CursoDetalle />} />
          <Route path="mensajes" element={<Mensajes />} />
          <Route element={<RutaProtegida roles={['admin']} />}>
            <Route path="categorias" element={<Categorias />} />
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<PublicLayout />}>
        <Route path="*" element={<NoEncontrada />} />
      </Route>
    </Routes>
  )
}
