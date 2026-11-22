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
  return <PaginaPublica etiqueta="Funcionalidades" titulo="El trabajo del curso, en un solo espacio." descripcion="Explora las herramientas conectadas al catálogo, contenido y comunicación que ofrece la base de datos actual."><Features /></PaginaPublica>
}

export function ContactoPage() {
  return (
    <PaginaPublica etiqueta="Contacto" titulo="Estamos para acompañar tu espacio académico." descripcion="Para consultas sobre acceso, cuentas o cursos, comunícate con el administrador de HomeClass de tu institución.">
      <section className="landing-seccion" aria-labelledby="titulo-ayuda">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Ayuda práctica</p>
          <h2 id="titulo-ayuda">Encuentra el siguiente paso según lo que necesitas.</h2>
          <p>HomeClass usa las cuentas y asociaciones académicas registradas por tu institución. Estas recomendaciones te ayudan a identificar quién puede revisar cada situación.</p>
        </div>
        <div className="landing-ayuda-grid">
          <article className="landing-ayuda-card">
            <span className="landing-detalle-icono"><i className="bi bi-person-lock" /></span>
            <h3>No puedes ingresar</h3>
            <p>Confirma que el correo esté escrito correctamente y que tu cuenta siga activa. El restablecimiento de contraseña no está habilitado en la API actual; solicita apoyo al administrador institucional.</p>
          </article>
          <article className="landing-ayuda-card">
            <span className="landing-detalle-icono"><i className="bi bi-journal-x" /></span>
            <h3>No aparece un curso</h3>
            <p>La administración puede revisar el estado del curso, la matrícula del estudiante o la asociación del docente con el curso.</p>
          </article>
          <article className="landing-ayuda-card">
            <span className="landing-detalle-icono"><i className="bi bi-collection-play" /></span>
            <h3>Tienes dudas sobre el contenido</h3>
            <p>Consulta con la persona docente del curso si faltan secciones, lecciones o recursos que deberían estar disponibles.</p>
          </article>
          <article className="landing-ayuda-card">
            <span className="landing-detalle-icono"><i className="bi bi-chat-dots" /></span>
            <h3>Necesitas comunicarte</h3>
            <p>Usa el foro del curso para conversaciones compartidas o la mensajería privada para contactar a otra cuenta.</p>
          </article>
          <article className="landing-ayuda-card">
            <span className="landing-detalle-icono"><i className="bi bi-clock-history" /></span>
            <h3>Tu sesión dejó de estar activa</h3>
            <p>Vuelve a ingresar con tu cuenta. Si no puedes iniciar sesión o el problema se repite, pide al administrador que compruebe el estado de la cuenta.</p>
          </article>
          <article className="landing-ayuda-card">
            <span className="landing-detalle-icono"><i className="bi bi-check2-circle" /></span>
            <h3>No ves el progreso esperado</h3>
            <p>El registro de progreso está disponible para estudiantes en sus cursos. Comprueba que ingresaste con la cuenta correcta y que puedes acceder a la lección.</p>
          </article>
          <article className="landing-ayuda-card">
            <span className="landing-detalle-icono"><i className="bi bi-chat-square-text" /></span>
            <h3>No puedes publicar en un foro</h3>
            <p>Las publicaciones requieren acceso al curso y que el foro siga abierto. Consulta con la persona docente si el tema está cerrado o archivado.</p>
          </article>
        </div>
      </section>
      <section className="landing-seccion" aria-labelledby="titulo-solicitud-ayuda">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Antes de solicitar ayuda</p>
          <h2 id="titulo-solicitud-ayuda">Comparte el contexto necesario, no tus credenciales.</h2>
          <p>Una descripción clara ayuda a que el equipo institucional identifique si se trata de la cuenta, el curso o una acción específica.</p>
        </div>
        <div className="landing-detalle-grid">
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-person-badge" /></span>
            <h3>Identifica tu perfil</h3>
            <p>Indica si utilizas HomeClass como administración, docente o estudiante y señala el curso relacionado, si aplica.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-list-task" /></span>
            <h3>Describe los pasos</h3>
            <p>Explica qué opción abriste, qué esperabas que ocurriera y qué mensaje o resultado apareció.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-shield-lock" /></span>
            <h3>Cuida tu información</h3>
            <p>No compartas tu contraseña, token de sesión ni datos privados de otras personas. Si adjuntas una captura, oculta la información sensible.</p>
          </article>
        </div>
      </section>
      <section className="contacto-canal">
        <span className="landing-gestion-icono"><i className="bi bi-building-check" /></span>
        <div><h2>Canal de atención institucional</h2><p>El administrador de tu institución gestiona invitaciones, permisos y cursos. Si ya tienes cuenta, ingresa a HomeClass para continuar.</p></div>
        <div className="landing-contacto-acciones">
          <Link className="btn btn-landing" to="/ingresar">Ir a ingresar <i className="bi bi-arrow-right" /></Link>
          <Link className="landing-enlace" to="/gestiones">Ver herramientas por perfil</Link>
        </div>
      </section>
    </PaginaPublica>
  )
}