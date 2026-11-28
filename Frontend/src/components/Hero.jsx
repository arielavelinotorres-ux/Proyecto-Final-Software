import { Link } from 'react-router-dom'
import IconoHomeClass from './IconoHomeClass.jsx'

export default function Hero() {
  return (
    <section className="landing-hero" aria-labelledby="titulo-inicio">
      <div className="landing-copy">
        <p className="landing-eyebrow"><IconoHomeClass tipo="marca" /> Entorno académico integral</p>
        <h1 id="titulo-inicio">La vida académica, <span>en un mismo lugar.</span></h1>
        <p>
          HomeClass organiza cursos, contenidos académicos y comunicación entre docentes y estudiantes.
          La información se consulta en la base de datos a través del backend.
        </p>
        <div className="landing-acciones">
          <Link className="btn btn-landing" to="/ingresar">Ingresar a HomeClass <i className="bi bi-arrow-right" /></Link>
          <Link className="landing-enlace" to="/funciones">Explorar funcionalidades <i className="bi bi-chevron-down" /></Link>
        </div>
        <div className="landing-resumen" aria-label="Áreas que integra HomeClass">
          <span><i className="bi bi-journal-bookmark" /> Cursos</span>
          <span><i className="bi bi-collection-play" /> Contenido</span>
          <span><i className="bi bi-chat-square-text" /> Comunicación</span>
        </div>
      </div>

      <div className="landing-preview" aria-label="Áreas que integra HomeClass">
        <div className="landing-preview-bar" aria-hidden="true">
          <span className="landing-preview-brand"><IconoHomeClass tipo="marca" /> HOMECLASS</span>
          <span className="landing-preview-status"><i className="bi bi-circle-fill" /> ESPACIO ACADÉMICO</span>
        </div>
        <div className="landing-preview-heading">
          <p className="landing-eyebrow">Campus digital</p>
          <h2>Todo tu aprendizaje, conectado.</h2>
        </div>
        <div className="landing-hero-resumen">
          <Link to="/funciones#funcionalidades">
            <article><span><IconoHomeClass tipo="cursos" /></span><h3>Catálogo académico</h3><p>Cursos, categorías y matrículas en un mismo lugar.</p><i className="bi bi-arrow-up-right landing-preview-arrow" aria-hidden="true" /></article>
          </Link>
          <Link to="/funciones#funcionalidades">
            <article><span><IconoHomeClass tipo="contenido" /></span><h3>Contenido</h3><p>Secciones, lecciones, recursos y progreso guardados.</p><i className="bi bi-arrow-up-right landing-preview-arrow" aria-hidden="true" /></article>
          </Link>
          <Link to="/funciones#funcionalidades">
            <article><span><IconoHomeClass tipo="comunicacion" /></span><h3>Comunicación</h3><p>Foros y mensajes privados entre usuarios.</p><i className="bi bi-arrow-up-right landing-preview-arrow" aria-hidden="true" /></article>
          </Link>
        </div>
        <div className="landing-preview-foot" aria-hidden="true">
          <span><i className="bi bi-shield-check" /> Aprendizaje organizado</span>
          <span><i className="bi bi-lightning-charge-fill" /> Una experiencia, múltiples espacios</span>
        </div>
      </div>
    </section>
  )
}
