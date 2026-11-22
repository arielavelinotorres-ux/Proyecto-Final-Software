import { Outlet, useLocation } from 'react-router-dom'
import Navbar from '../components/Navbar.jsx'
import Footer from '../components/Footer.jsx'

// Estructura de las páginas públicas. <Outlet /> es el hueco donde Router pone la página.
export default function PublicLayout() {
  const { pathname } = useLocation()
  const esIngreso = pathname === '/ingresar'

  return (
    <>
      <Navbar />
      <main className={esIngreso ? 'login-page' : 'publico'}>
        <Outlet />
      </main>
      <Footer />
    </>
  )
}
