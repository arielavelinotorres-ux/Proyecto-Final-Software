import { funcionalidades } from '../data/funcionalidades.js'

export default function Features() {
  return (
    <section className="landing-seccion" id="funcionalidades" aria-labelledby="titulo-func">
      <div className="landing-seccion-cabecera">
        <p className="landing-eyebrow">Una plataforma, tareas conectadas</p>
        <h2 id="titulo-func">Todo lo necesario para acompañar el aprendizaje.</h2>
        <p>Herramientas prácticas para organizar el trabajo académico y tener una visión más completa de cada curso.</p>
      </div>
      <div className="landing-funciones">
        {funcionalidades.map((f) => (
          <FeatureCard key={f.id} {...f} />
        ))}
      </div>
    </section>
  )
}

function FeatureCard({ icono, titulo, texto, imagen, alt }) {
  return (
    <article className="landing-funcion">
      <img className="landing-funcion-imagen" src={imagen} alt={alt} loading="lazy" />
      <span className="landing-funcion-icono"><i className={`bi ${icono}`} /></span>
      <h3>{titulo}</h3>
      <p>{texto}</p>
    </article>
  )
}
