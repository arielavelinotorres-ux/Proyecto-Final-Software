export default function Audiencias() {
  return (
    <section className="landing-seccion landing-proposito" aria-labelledby="titulo-proposito">
      <div className="landing-seccion-cabecera">
        <p className="landing-eyebrow">¿Para qué sirve HomeClass?</p>
        <h2 id="titulo-proposito">Para organizar cursos, contenidos y comunicación académica.</h2>
        <p>HomeClass usa el backend para consultar y actualizar la información existente en SQL Server. Está dirigida a instituciones educativas y a las personas que participan en ellas.</p>
      </div>
      <ul className="landing-proposito-puntos">
        <li><i className="bi bi-journal-check" /><span><strong>Organiza los cursos</strong><small>Administra el catálogo, las categorías y las matrículas.</small></span></li>
        <li><i className="bi bi-graph-up-arrow" /><span><strong>Da seguimiento al contenido</strong><small>Consulta lecciones y registra el progreso académico.</small></span></li>
        <li><i className="bi bi-people" /><span><strong>Conecta a la comunidad educativa</strong><small>Ofrece herramientas según las responsabilidades de cada cuenta.</small></span></li>
      </ul>
    </section>
  )
}