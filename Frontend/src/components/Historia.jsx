export default function Historia() {
  return (
    <>
      <section className="landing-seccion landing-historia" aria-labelledby="titulo-historia">
        <div className="landing-historia-intro">
          <p className="landing-eyebrow">Origen y propósito</p>
          <h2 id="titulo-historia">Una propuesta para hacer más claro el trabajo académico.</h2>
        </div>
        <div className="landing-historia-cuerpo">
          <p>HomeClass nació como proyecto de Ingeniería de Software con una pregunta concreta: ¿cómo hacer más fácil el seguimiento de cursos para quienes enseñan, aprenden y coordinan?</p>
          <p>La propuesta fue reunir en un mismo lugar el catálogo, el contenido académico, el progreso y la comunicación. Así, cada persona puede encontrar las actividades relacionadas con su curso sin depender de herramientas desconectadas entre sí.</p>
          <p>El proyecto se apoya en una base de datos relacional y una API que sirve la información al frontend. Los perfiles determinan qué cursos y acciones están disponibles para cada cuenta.</p>
        </div>
      </section>

      <section className="landing-seccion" aria-labelledby="titulo-enfoque">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Enfoque del proyecto</p>
          <h2 id="titulo-enfoque">Del problema académico a una experiencia organizada.</h2>
          <p>El enfoque de HomeClass se entiende a través de tres decisiones: ordenar la información, separar responsabilidades y mantener visibles las funciones que están conectadas.</p>
        </div>
        <div className="landing-detalle-grid">
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-diagram-3" /></span>
            <h3>Información relacionada</h3>
            <p>Los cursos se asocian con categorías, docentes, matrículas y contenido para que la información tenga contexto.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-person-lock" /></span>
            <h3>Responsabilidades claras</h3>
            <p>La navegación y las acciones se adaptan al rol de la cuenta y a las asociaciones académicas registradas.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-database-check" /></span>
            <h3>Alcance verificable</h3>
            <p>La información de cursos, contenido y comunicación proviene de operaciones respaldadas por la API activa.</p>
          </article>
        </div>
      </section>

      <section className="landing-seccion" aria-labelledby="titulo-construccion">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Cómo se conecta</p>
          <h2 id="titulo-construccion">De los datos académicos a las acciones de cada persona.</h2>
          <p>La solución combina almacenamiento estructurado, reglas de acceso y una interfaz para presentar la información con el contexto necesario.</p>
        </div>
        <div className="landing-detalle-grid">
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-database" /></span>
            <h3>Datos relacionados</h3>
            <p>La base conecta categorías y cursos con docentes y matrículas, y vincula el contenido con su curso y sus lecciones.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-arrow-left-right" /></span>
            <h3>Servicios de la plataforma</h3>
            <p>La API consulta o actualiza esos datos, comprueba la sesión y aplica las reglas correspondientes antes de responder.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-window" /></span>
            <h3>Interfaz por perfil</h3>
            <p>El frontend organiza el acceso a cursos, contenido y comunicación según el rol y la relación de la cuenta con el curso.</p>
          </article>
        </div>
      </section>

      <section className="landing-seccion" aria-labelledby="titulo-alcance-historia">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Alcance actual</p>
          <h2 id="titulo-alcance-historia">La propuesta se concentra en el trabajo cotidiano del curso.</h2>
          <p>El sistema activo prioriza tareas que tienen un recorrido completo entre la interfaz, el backend y los datos persistidos.</p>
        </div>
        <div className="landing-detalle-grid">
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-journal-check" /></span>
            <h3>Organización</h3>
            <p>Consulta de cursos y gestión autorizada de categorías, cursos y matrículas.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-book-half" /></span>
            <h3>Aprendizaje</h3>
            <p>Consulta de secciones, lecciones y recursos, con actualización del progreso de estudiantes.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-chat-dots" /></span>
            <h3>Interacción</h3>
            <p>Foros por curso, publicaciones y respuestas, además de mensajes privados entre cuentas.</p>
          </article>
        </div>
        <div className="landing-nota-alcance">
          <i className="bi bi-info-circle" aria-hidden="true" />
          <p>La página distingue estas funciones disponibles de módulos como asistencia, calificaciones y evaluaciones, que no tienen rutas activas completas en la API actual.</p>
        </div>
      </section>
    </>
  )
}
