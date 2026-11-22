import Hero from '../components/Hero.jsx'
import { Link } from 'react-router-dom'

export default function Inicio() {
  return (
    <div className="inicio">
      <Hero />
      <section className="home-overview" aria-labelledby="home-overview-title">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Explora HomeClass</p>
          <h2 id="home-overview-title">Un espacio para aprender, enseñar y coordinar.</h2>
          <p>HomeClass conecta el catálogo académico con el contenido de cada curso y la comunicación entre sus participantes. Revisa cada sección para conocer los detalles.</p>
        </div>
        <div className="home-overview-grid">
          <Link className="home-overview-card" to="/nosotros">
            <span className="home-overview-icono"><i className="bi bi-people" /></span>
            <span className="home-overview-etiqueta">La plataforma</span>
            <strong>Conoce HomeClass</strong>
            <span>Su propósito y las personas a quienes conecta.</span>
            <i className="bi bi-arrow-right home-overview-flecha" aria-hidden="true" />
          </Link>
          <Link className="home-overview-card" to="/historia">
            <span className="home-overview-icono"><i className="bi bi-book" /></span>
            <span className="home-overview-etiqueta">El proyecto</span>
            <strong>Historia y enfoque</strong>
            <span>El problema académico que busca resolver.</span>
            <i className="bi bi-arrow-right home-overview-flecha" aria-hidden="true" />
          </Link>
          <Link className="home-overview-card" to="/gestiones">
            <span className="home-overview-icono"><i className="bi bi-person-workspace" /></span>
            <span className="home-overview-etiqueta">Por perfil</span>
            <strong>Herramientas y tareas</strong>
            <span>Qué puede hacer administración, docencia y estudiantes.</span>
            <i className="bi bi-arrow-right home-overview-flecha" aria-hidden="true" />
          </Link>
          <Link className="home-overview-card" to="/funciones">
            <span className="home-overview-icono"><i className="bi bi-grid-1x2" /></span>
            <span className="home-overview-etiqueta">Funciones</span>
            <strong>Cursos, contenido y comunicación</strong>
            <span>Explora las operaciones conectadas al backend.</span>
            <i className="bi bi-arrow-right home-overview-flecha" aria-hidden="true" />
          </Link>
          <Link className="home-overview-card" to="/contacto">
            <span className="home-overview-icono"><i className="bi bi-life-preserver" /></span>
            <span className="home-overview-etiqueta">Ayuda</span>
            <strong>Acceso y soporte</strong>
            <span>Pasos para resolver dudas sobre tu cuenta o cursos.</span>
            <i className="bi bi-arrow-right home-overview-flecha" aria-hidden="true" />
          </Link>
        </div>
      </section>
      <section className="inicio-cierre" aria-labelledby="titulo-cierre">
        <div>
          <p className="landing-eyebrow">Explora HomeClass</p>
          <h2 id="titulo-cierre">Una plataforma para organizar el aprendizaje.</h2>
          <p>Conoce quiénes somos y qué puede hacer cada perfil en páginas dedicadas.</p>
        </div>
        <div className="landing-acciones">
          <Link className="btn btn-landing" to="/funciones">Ver funcionalidades <i className="bi bi-arrow-right" /></Link>
          <Link className="landing-enlace" to="/nosotros">Conocer HomeClass</Link>
        </div>
      </section>
    </div>
  )
}
