const PERFILES = [
  {
    id: 'gestion-admin',
    clase: 'admin',
    icono: 'bi-shield-check',
    nombre: 'Administración',
    resumen: 'Coordina el catálogo académico y las matrículas.',
    detalle: 'Prepara la estructura que permite encontrar los cursos y vincular cada espacio con una persona docente y con sus estudiantes.',
    tareas: ['Crear y actualizar categorías y subcategorías.', 'Crear cursos y asignarles una subcategoría.', 'Asignar docentes y mantener la información del curso.', 'Matricular estudiantes y revisar sus asociaciones.', 'Administrar el catálogo con las opciones autorizadas.'],
  },
  {
    id: 'gestion-docente',
    clase: 'docente',
    icono: 'bi-easel2',
    nombre: 'Docencia',
    resumen: 'Gestiona el contenido y la conversación de sus cursos.',
    detalle: 'Trabaja sobre los cursos asociados a su cuenta: consulta el contenido y acompaña la actividad mediante los foros y la mensajería.',
    tareas: ['Consultar los cursos asociados a su cuenta.', 'Revisar secciones, lecciones y recursos del curso.', 'Crear, actualizar y archivar foros según sus permisos.', 'Publicar aportes y responder en conversaciones.', 'Enviar y recibir mensajes privados.'],
  },
  {
    id: 'gestion-estudiante',
    clase: 'estudiante',
    icono: 'bi-mortarboard',
    nombre: 'Estudiantes',
    resumen: 'Consulta el contenido y participa en sus cursos matriculados.',
    detalle: 'Accede a los cursos donde tiene matrícula activa, revisa sus recursos y puede registrar el avance de las lecciones.',
    tareas: ['Consultar los cursos en los que está matriculado.', 'Revisar el contenido y los recursos de cada curso.', 'Registrar el avance de las lecciones disponibles.', 'Participar en los foros del curso.', 'Enviar y recibir mensajes privados.'],
  },
]

export default function Gestiones() {
  return (
    <>
      <section className="landing-seccion" id="gestiones" aria-labelledby="titulo-gestiones">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Gestiones por perfil</p>
          <h2 id="titulo-gestiones">Cada rol, con las herramientas que necesita.</h2>
          <p>HomeClass separa las acciones de administración, docencia y estudiante. La API valida el rol y las asociaciones con cursos antes de mostrar o permitir cada operación.</p>
        </div>
        <div className="landing-gestiones">
          {PERFILES.map((perfil) => (
            <article className={`landing-gestion landing-gestion-${perfil.clase}`} key={perfil.id}>
              <span className="landing-gestion-icono"><i className={`bi ${perfil.icono}`} /></span>
              <h3>{perfil.nombre}</h3>
              <p>{perfil.resumen}</p>
              <p className="landing-gestion-detalle">{perfil.detalle}</p>
              <ul>{perfil.tareas.map((tarea) => <li key={tarea}>{tarea}</li>)}</ul>
            </article>
          ))}
        </div>
        <div className="landing-gestiones-nota">
          <span><i className="bi bi-info-circle" aria-hidden="true" /></span>
          <p><strong>El acceso depende de la cuenta.</strong> El estudiantado ve los cursos donde está matriculado y participa en el contenido y la comunicación; la docencia trabaja con cursos asociados a su perfil y la administración mantiene el catálogo. Las opciones no conectadas al backend no se presentan como disponibles.</p>
        </div>
      </section>

      <section className="landing-seccion landing-pasos" aria-labelledby="titulo-flujo-perfiles">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Cómo colaboran los perfiles</p>
          <h2 id="titulo-flujo-perfiles">Un curso conecta la organización, la enseñanza y el aprendizaje.</h2>
          <p>Las responsabilidades son distintas, pero forman parte de un mismo recorrido académico.</p>
        </div>
        <ol>
          <li><span>01</span><div><h3>Organizar la oferta</h3><p>Administración crea las categorías y cursos, asigna docentes y registra las matrículas autorizadas.</p></div></li>
          <li><span>02</span><div><h3>Preparar la interacción</h3><p>La docencia consulta el curso, revisa su contenido y puede mantener los foros disponibles para sus participantes.</p></div></li>
          <li><span>03</span><div><h3>Participar y avanzar</h3><p>El estudiante matriculado consulta las lecciones y recursos, registra progreso y participa en las conversaciones abiertas.</p></div></li>
        </ol>
      </section>
    </>
  )
}