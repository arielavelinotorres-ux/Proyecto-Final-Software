const PERFILES = [
  {
    id: 'gestion-admin',
    clase: 'admin',
    icono: 'bi-shield-check',
    nombre: 'Administración',
    resumen: 'Coordina el catálogo académico y las matrículas.',
    tareas: ['Crear categorías y subcategorías', 'Crear y organizar cursos', 'Matricular estudiantes'],
  },
  {
    id: 'gestion-docente',
    clase: 'docente',
    icono: 'bi-easel2',
    nombre: 'Docencia',
    resumen: 'Gestiona el contenido y la conversación de sus cursos.',
    tareas: ['Consultar secciones y lecciones', 'Crear foros y publicaciones', 'Enviar y recibir mensajes'],
  },
  {
    id: 'gestion-estudiante',
    clase: 'estudiante',
    icono: 'bi-mortarboard',
    nombre: 'Estudiantes',
    resumen: 'Consulta el contenido y participa en sus cursos matriculados.',
    tareas: ['Consultar lecciones y recursos', 'Registrar el progreso de aprendizaje', 'Participar en foros y mensajes'],
  },
]

export default function Gestiones() {
  return (
    <section className="landing-seccion" id="gestiones" aria-labelledby="titulo-gestiones">
      <div className="landing-seccion-cabecera">
        <p className="landing-eyebrow">Gestiones por perfil</p>
        <h2 id="titulo-gestiones">Cada rol, con las herramientas que necesita.</h2>
        <p>Las acciones disponibles cambian según las responsabilidades de cada cuenta y los permisos que la institución le asigna.</p>
      </div>
      <div className="landing-gestiones">
        {PERFILES.map((perfil) => (
          <article className={`landing-gestion landing-gestion-${perfil.clase}`} key={perfil.id}>
            <span className="landing-gestion-icono"><i className={`bi ${perfil.icono}`} /></span>
            <h3>{perfil.nombre}</h3>
            <p>{perfil.resumen}</p>
            <ul>{perfil.tareas.map((tarea) => <li key={tarea}>{tarea}</li>)}</ul>
          </article>
        ))}
      </div>
    </section>
  )
}