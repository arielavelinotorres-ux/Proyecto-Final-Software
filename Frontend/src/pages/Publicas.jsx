import Audiencias from '../components/Audiencias.jsx'
import Features from '../components/Features.jsx'
import Gestiones from '../components/Gestiones.jsx'
import Historia from '../components/Historia.jsx'
import { Link } from 'react-router-dom'

function PaginaPublica({ etiqueta, titulo, descripcion, children }) {
  return (
    <div className="inicio pagina-publica">
      <header className="pagina-publica-intro">
        <p className="landing-eyebrow">{etiqueta}</p>
        <h1>{titulo}</h1>
        <p>{descripcion}</p>
      </header>
      {children}
    </div>
  )
}

export function NosotrosPage() {
  return <PaginaPublica etiqueta="Quiénes somos" titulo="Una comunidad académica, mejor conectada." descripcion="HomeClass reúne la información y las herramientas que necesitan las personas que enseñan, aprenden y coordinan."><Audiencias /></PaginaPublica>
}

export function HistoriaPage() {
  return <PaginaPublica etiqueta="Nuestra historia" titulo="Una idea nacida de una necesidad académica." descripcion="HomeClass es un proyecto de Ingeniería de Software enfocado en hacer más claro el seguimiento de cursos."><Historia /></PaginaPublica>
}

export function GestionesPage() {
  return <PaginaPublica etiqueta="Gestiones" titulo="Herramientas para cada responsabilidad." descripcion="Las acciones disponibles se organizan según el perfil y los permisos asignados por la institución."><Gestiones /></PaginaPublica>
}

export function FuncionesPage() {
  return <PaginaPublica etiqueta="Funcionalidades" titulo="El trabajo del curso, en un solo espacio." descripcion="Explora las herramientas para organizar cursos, registrar actividad y acompañar el progreso académico."><Features /></PaginaPublica>
}

export function ContactoPage() {
  return (
    <PaginaPublica etiqueta="Contacto" titulo="Estamos para acompañar tu espacio académico." descripcion="Para consultas sobre acceso, cuentas o cursos, comunícate con el administrador de HomeClass de tu institución.">
      <section className="contacto-canal">
        <span className="landing-gestion-icono"><i className="bi bi-building-check" /></span>
        <div><h2>Canal de atención institucional</h2><p>El administrador de tu institución gestiona invitaciones, permisos y cursos. Si ya tienes cuenta, ingresa a HomeClass para continuar.</p></div>
        <Link className="btn btn-landing" to="/ingresar">Ir a ingresar <i className="bi bi-arrow-right" /></Link>
      </section>
    </PaginaPublica>
  )
}