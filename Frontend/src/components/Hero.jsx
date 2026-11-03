import { Link } from 'react-router-dom'

export default function Hero() {
  return (
    <section className="landing-hero" aria-labelledby="titulo-inicio">
      <div className="landing-copy">
        <p className="landing-eyebrow"><i className="bi bi-mortarboard-fill" /> Entorno académico integral</p>
        <h1 id="titulo-inicio">La vida académica, en un mismo lugar.</h1>
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

      <div className="landing-hero-resumen" aria-label="Áreas que integra HomeClass">
        <article><span><i className="bi bi-journal-bookmark-fill" /></span><h2>Catálogo académico</h2><p>Cursos, categorías y matrículas desde SQL Server.</p></article>
        <article><span><i className="bi bi-collection-play-fill" /></span><h2>Contenido</h2><p>Secciones, lecciones, recursos y progreso guardados.</p></article>
        <article><span><i className="bi bi-chat-square-text-fill" /></span><h2>Comunicación</h2><p>Foros y mensajes privados entre usuarios.</p></article>
      </div>
    </section>
  )
}
