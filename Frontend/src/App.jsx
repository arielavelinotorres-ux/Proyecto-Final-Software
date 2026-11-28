import { Navigate, Route, Routes } from 'react-router-dom'
import { useAuth } from './context/AuthContext.jsx'
import PublicLayout from './layouts/PublicLayout.jsx'
import AppLayout from './layouts/AppLayout.jsx'
import RutaProtegida from './components/RutaProtegida.jsx'
import Inicio from './pages/Inicio.jsx'
import { ContactoPage, GestionesPage, NosotrosPage } from './pages/Publicas.jsx'
import Ingresar from './pages/Ingresar.jsx'
import NoEncontrada from './pages/NoEncontrada.jsx'
import Mensajes from './pages/Mensajes.jsx'
import PanelLms from './pages/PanelLms.jsx'
// Estructura del Nav
export default function App() {
  return (
    <Routes>
      <Route element={<PublicLayout />}>
        <Route index element={<Inicio />} />
        <Route path="nosotros" element={<NosotrosPage />} />
        <Route path="gestiones" element={<GestionesPage />} />
        <Route path="contacto" element={<ContactoPage />} />
        <Route path="ingresar" element={<Ingresar />} />
      </Route>

      <Route path="app" element={<RutaProtegida />}>
        <Route element={<AppLayout />}>
          <Route index element={<PanelLms />} />
          <Route path="mensajes" element={<Mensajes />} />
        </Route>
      </Route>

      <Route path="*" element={<PublicLayout />}>
        <Route path="*" element={<NoEncontrada />} />
      </Route>
    </Routes>
  )
}
