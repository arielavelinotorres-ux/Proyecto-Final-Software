import { funcionalidades } from '../data/funcionalidades.js'

export default function Features() {
  return (
    <>
      <section className="landing-seccion" id="funcionalidades" aria-labelledby="titulo-func">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Funciones conectadas al backend</p>
          <h2 id="titulo-func">Catálogo, contenido y comunicación.</h2>
          <p>Estas funciones usan la API activa para consultar o actualizar datos en SQL Server. La disponibilidad de cada operación depende del perfil, la sesión y la relación de la cuenta con el curso.</p>
        </div>
        <div className="landing-funciones">
          {funcionalidades.map((f) => (
            <FeatureCard key={f.id} {...f} />
          ))}
        </div>
      </section>

      <section className="landing-seccion" aria-labelledby="titulo-integracion">
        <div className="landing-seccion-cabecera">
          <p className="landing-eyebrow">Qué ocurre detrás de cada opción</p>
          <h2 id="titulo-integracion">Las acciones se consultan y guardan en la plataforma.</h2>
          <p>La interfaz no inventa cursos ni simula resultados: las opciones visibles usan la sesión activa para obtener o enviar información al backend.</p>
        </div>
        <div className="landing-detalle-grid">
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-search" /></span>
            <h3>Consultas con alcance</h3>
            <p>La lista de cursos, el detalle, el contenido y los foros se limitan a los espacios que la cuenta puede consultar.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-shield-check" /></span>
            <h3>Acciones autorizadas</h3>
            <p>La administración mantiene el catálogo, la docencia gestiona foros de sus cursos y el estudiante actualiza su propio progreso.</p>
          </article>
          <article className="landing-detalle-card">
            <span className="landing-detalle-icono"><i className="bi bi-arrow-repeat" /></span>
            <h3>Información persistente</h3>
            <p>Los cambios aceptados por la API se guardan en la base de datos; al volver a consultar, HomeClass presenta la información almacenada.</p>
          </article>
        </div>
        <div className="landing-nota-alcance">
          <i className="bi bi-info-circle" aria-hidden="true" />
          <p>Asistencia, calificaciones, evaluaciones y entregas no se anuncian como opciones activas porque no cuentan con un recorrido completo conectado a la API actual.</p>
        </div>
      </section>
    </>
  )
}

function FeatureCard({ icono, titulo, texto, puntos, imagen, alt }) {
  return (
    <article className="landing-funcion">
      <img className="landing-funcion-imagen" src={imagen} alt={alt} loading="lazy" />
      <span className="landing-funcion-icono"><i className={`bi ${icono}`} /></span>
      <h3>{titulo}</h3>
      <p>{texto}</p>
      <ul className="landing-funcion-detalles">
        {puntos.map((punto) => <li key={punto}>{punto}</li>)}
      </ul>
    </article>
  )
}
