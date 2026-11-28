import { Link } from 'react-router-dom'

export default function Hero() {
  return (
    <section className="landing-hero" aria-labelledby="titulo-inicio">
      <div className="landing-copy">
        <p className="landing-eyebrow">Entorno académico integral</p>
        <h1 id="titulo-inicio">La vida académica, <span>en un mismo lugar.</span></h1>
        <p>
          HomeClass organiza cursos, contenidos académicos y comunicación entre docentes y estudiantes.
          La información se consulta en la base de datos a través del backend.
        </p>
        <div className="landing-acciones">
          <Link className="btn btn-landing" to="/ingresar">Ingresar a HomeClass <i className="bi bi-arrow-right" /></Link>
        </div>
        <div className="landing-resumen" aria-label="Áreas que integra HomeClass">
          <span><i className="bi bi-journal-bookmark" /> Cursos</span>
          <span><i className="bi bi-collection-play" /> Contenido</span>
          <span><i className="bi bi-chat-square-text" /> Comunicación</span>
        </div>
      </div>

      <div className="landing-preview" aria-label="Áreas que integra HomeClass">
        <div className="landing-preview-bar" aria-hidden="true">
          <span className="landing-preview-brand"><img className="homeclass-logo-image" src="/homeclass-logo.jpg" alt="" /> HOMECLASS</span>
          <span className="landing-preview-status"><i className="bi bi-circle-fill" /> ESPACIO ACADÉMICO</span>
        </div>
        <div className="landing-preview-heading">
          <h2>Todo tu aprendizaje, conectado.</h2>
        </div>
        <div className="landing-hero-resumen">
          <article>
            <h3>Catálogo académico</h3>
            <p>Organiza cursos, categorías y matrículas en un solo lugar.</p>
          </article>
          <article>
            <h3>Contenido</h3>
            <p>Consulta secciones, lecciones y recursos, y sigue tu progreso.</p>
          </article>
          <article>
            <h3>Comunicación</h3>
            <p>Participa en foros y envía mensajes privados.</p>
          </article>
        </div>
        <div className="landing-preview-foot" aria-hidden="true">
          <span><i className="bi bi-shield-check" /> Aprendizaje organizado</span>
          <span><i className="bi bi-lightning-charge-fill" /> Una experiencia, múltiples espacios</span>
        </div>
      </div>
    </section>
  )
}
