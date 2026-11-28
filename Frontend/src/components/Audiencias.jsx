export default function Audiencias() {
  return (
    <>
      <section className="landing-seccion landing-proposito" aria-labelledby="titulo-proposito">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">¿Para qué sirve HomeClass?</p>
          <h2 id="titulo-proposito">Para organizar cursos, contenidos y comunicación académica.</h2>
          <p>HomeClass conecta el catálogo académico con el trabajo que ocurre dentro de cada curso. La información se consulta y actualiza mediante la API conectada a SQL Server.</p>
        </div>
        <ul className="landing-proposito-puntos">
          <li><span><strong>Organiza los cursos</strong><small>Administra el catálogo, las categorías y las matrículas.</small></span></li>
          <li><span><strong>Da seguimiento al contenido</strong><small>Consulta lecciones, recursos y progreso académico.</small></span></li>
          <li><span><strong>Conecta a la comunidad educativa</strong><small>Ofrece herramientas según las responsabilidades de cada cuenta.</small></span></li>
        </ul>
      </section>

      <section className="landing-seccion" aria-labelledby="titulo-comunidad">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Una experiencia compartida</p>
          <h2 id="titulo-comunidad">El mismo espacio acompaña distintas responsabilidades.</h2>
          <p>El catálogo y la información del curso se organizan por cuenta, de modo que cada persona encuentre las herramientas relacionadas con su participación.</p>
        </div>
        <div className="landing-detalle-grid">
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-person-gear" /></span>
            <h3>Coordinación institucional</h3>
            <p>La administración mantiene organizados cursos, categorías y matrículas desde las opciones autorizadas.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-easel2" /></span>
            <h3>Enseñanza y acompañamiento</h3>
            <p>La docencia consulta el contenido de sus cursos, participa en foros y mantiene comunicación con sus estudiantes.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-mortarboard" /></span>
            <h3>Aprendizaje y participación</h3>
            <p>El estudiantado accede a sus cursos, revisa recursos, actualiza el progreso y participa en la comunicación académica.</p>
          </article>
        </div>
      </section>

      <section className="landing-seccion landing-pasos" aria-labelledby="titulo-recorrido">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Cómo empezar</p>
          <h2 id="titulo-recorrido">Un recorrido claro desde el acceso hasta el curso.</h2>
        </div>
        <ol>
          <li><span>01</span><div><h3>Ingresa con tu cuenta</h3><p>El acceso utiliza el correo y la contraseña de una cuenta activa.</p></div></li>
          <li><span>02</span><div><h3>Abre las opciones de tu perfil</h3><p>El menú muestra las páginas y acciones disponibles para tu rol.</p></div></li>
          <li><span>03</span><div><h3>Continúa el trabajo del curso</h3><p>Consulta el contenido y usa las herramientas de progreso y comunicación conectadas.</p></div></li>
        </ol>
      </section>

      <section className="landing-seccion" aria-labelledby="titulo-experiencia">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Una experiencia con contexto</p>
          <h2 id="titulo-experiencia">Cada herramienta se relaciona con el curso y con tu participación.</h2>
          <p>En lugar de presentar las mismas acciones a todas las cuentas, HomeClass organiza la información según el curso y el vínculo académico de cada persona.</p>
        </div>
        <div className="landing-detalle-grid">
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-folder2-open" /></span>
            <h3>Una estructura reconocible</h3>
            <p>Las categorías agrupan cursos; cada curso relaciona a su docente y a las matrículas de estudiantes. Esto ayuda a ubicar los espacios académicos que corresponden a cada cuenta.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-list-check" /></span>
            <h3>Contenido en secuencia</h3>
            <p>Las secciones organizan lecciones y recursos. El estudiante puede consultar ese recorrido y guardar el estado de avance de las lecciones disponibles.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-chat-square-dots" /></span>
            <h3>Comunicación vinculada</h3>
            <p>Los foros reúnen publicaciones alrededor de un curso y la mensajería permite mantener conversaciones privadas con otros usuarios activos.</p>
          </article>
        </div>
      </section>
    </>
  )
}