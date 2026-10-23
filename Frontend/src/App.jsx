import { Navigate, Routes, Route } from 'react-router-dom'
import { useAuth } from './context/AuthContext.jsx'
import PublicLayout from './layouts/PublicLayout.jsx'
import AppLayout from './layouts/AppLayout.jsx'
import RutaProtegida from './components/RutaProtegida.jsx'
import Inicio from './pages/Inicio.jsx'
import { ContactoPage, FuncionesPage, GestionesPage, HistoriaPage, NosotrosPage } from './pages/Publicas.jsx'
import Ingresar from './pages/Ingresar.jsx'
import NoEncontrada from './pages/NoEncontrada.jsx'
import Resumen from './pages/Resumen.jsx'
import Cursos from './pages/Cursos.jsx'
import CursoDetalle from './pages/CursoDetalle.jsx'
import Usuarios from './pages/admin/Usuarios.jsx'
import Perfil from './pages/Perfil.jsx'
import Agenda from './pages/Agenda.jsx'
import Historial from './pages/estudiante/Historial.jsx'
import Reportes from './pages/admin/Reportes.jsx'
import Auditoria from './pages/admin/Auditoria.jsx'
import CentroAcademico from './pages/CentroAcademico.jsx'
import { tienePermisoAdmin } from './utils/permisos.js'

// RUTAS. Cada <Route> dice "cuando la dirección sea X, muestra la página Y".
//  - Públicas: /, /ingresar
//  - Privadas (con sesión): todo lo que cuelga de /app. /app/usuarios solo para administración.
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
          <Route index element={<InicioDashboard />} />
          <Route element={<RutaProtegida permisosAdmin={['cursos']} />}>
            <Route path="cursos" element={<Cursos />} />
            <Route path="cursos/:id" element={<CursoDetalle />} />
          </Route>
          <Route path="perfil" element={<Perfil />} />
          <Route element={<RutaProtegida roles={['admin']} permisosAdmin={['usuarios']} />}>
            <Route path="usuarios" element={<Usuarios />} />
          </Route>
          <Route element={<RutaProtegida roles={['admin']} permisosAdmin={['usuarios.restablecer']} />}>
            <Route path="seguridad" element={<Usuarios modoSeguridad />} />
          </Route>
          <Route element={<RutaProtegida roles={['admin']} permisosAdmin={['reportes']} />}>
            <Route path="reportes" element={<Reportes />} />
          </Route>
          <Route element={<RutaProtegida roles={['admin']} permisosAdmin={['auditoria.consultar']} />}>
            <Route path="auditoria" element={<Auditoria />} />
          </Route>
          <Route element={<RutaProtegida roles={['docente', 'estudiante']} />}>
            <Route path="agenda" element={<Agenda />} />
          </Route>
          <Route element={<RutaProtegida roles={['estudiante']} />}>
            <Route path="historial" element={<Historial />} />
          </Route>
          <Route element={<RutaProtegida roles={['docente', 'estudiante']} />}>
            <Route path="herramientas" element={<CentroAcademico />} />
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<PublicLayout />}>
        <Route path="*" element={<NoEncontrada />} />
      </Route>
    </Routes>
  )
}

function InicioDashboard() {
  const { usuario } = useAuth()
  if (usuario.rol === 'admin' && !tienePermisoAdmin(usuario, 'general')) {
    const destino = tienePermisoAdmin(usuario, 'usuarios') ? '/app/usuarios'
      : tienePermisoAdmin(usuario, 'cursos') ? '/app/cursos'
        : tienePermisoAdmin(usuario, 'reportes') ? '/app/reportes'
          : tienePermisoAdmin(usuario, 'auditoria.consultar') ? '/app/auditoria'
            : '/app/perfil'
    return <Navigate to={destino} replace />
  }
  return <Resumen />
}
