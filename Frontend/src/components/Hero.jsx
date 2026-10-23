import { Link } from 'react-router-dom'

export default function Hero() {
  return (
    <section className="landing-hero" aria-labelledby="titulo-inicio">
      <div className="landing-copy">
        <p className="landing-eyebrow"><i className="bi bi-mortarboard-fill" /> Entorno académico integral</p>
        <h1 id="titulo-inicio">La vida académica, en un mismo lugar.</h1>
        <p>
          HomeClass conecta la gestión de cursos, el seguimiento del aprendizaje y el trabajo diario de docentes y estudiantes.
          Información clara para que cada persona sepa qué ocurre y qué sigue.
        </p>
        <div className="landing-acciones">
          <Link className="btn btn-landing" to="/ingresar">Ingresar a HomeClass <i className="bi bi-arrow-right" /></Link>
          <Link className="landing-enlace" to="/funciones">Explorar funcionalidades <i className="bi bi-chevron-down" /></Link>
        </div>
        <div className="landing-resumen" aria-label="Áreas que integra HomeClass">
          <span><i className="bi bi-journal-check" /> Cursos</span>
          <span><i className="bi bi-clipboard-data" /> Seguimiento</span>
          <span><i className="bi bi-people" /> Comunidad educativa</span>
        </div>
      </div>

      <div className="landing-hero-resumen" aria-label="Áreas que integra HomeClass">
        <article><span><i className="bi bi-journal-bookmark-fill" /></span><h2>Gestión académica</h2><p>Cursos, evaluaciones y materiales organizados por espacio.</p></article>
        <article><span><i className="bi bi-clipboard-data-fill" /></span><h2>Seguimiento</h2><p>Calificaciones, asistencia y entregas en un mismo entorno.</p></article>
        <article><span><i className="bi bi-people-fill" /></span><h2>Trabajo coordinado</h2><p>Una herramienta compartida por la institución educativa.</p></article>
      </div>
    </section>
  )
}
