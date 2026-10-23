const PERFILES = [
  {
    id: 'gestion-admin',
    clase: 'admin',
    icono: 'bi-shield-check',
    nombre: 'Administración',
    resumen: 'Coordina la operación institucional y delega responsabilidades.',
    tareas: ['Crear y organizar cursos', 'Administrar cuentas y permisos', 'Consultar reportes y auditoría'],
  },
  {
    id: 'gestion-docente',
    clase: 'docente',
    icono: 'bi-easel2',
    nombre: 'Docencia',
    resumen: 'Gestiona el trabajo del curso y acompaña el progreso del grupo.',
    tareas: ['Registrar notas y asistencia', 'Publicar evaluaciones y materiales', 'Revisar y calificar entregas'],
  },
  {
    id: 'gestion-estudiante',
    clase: 'estudiante',
    icono: 'bi-mortarboard',
    nombre: 'Estudiantes',
    resumen: 'Consulta su actividad y participa en cada curso desde un mismo sitio.',
    tareas: ['Revisar notas y asistencia', 'Entregar trabajos y consultar fechas', 'Descargar materiales y leer avisos'],
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