import Hero from '../components/Hero.jsx'
import { Link } from 'react-router-dom'

export default function Inicio() {
  return (
    <div className="inicio">
      <Hero />
      <section className="inicio-cierre" aria-labelledby="titulo-cierre">
        <div>
          <p className="landing-eyebrow">Explora HomeClass</p>
          <h2 id="titulo-cierre">Una plataforma para organizar el aprendizaje.</h2>
          <p>Conoce quiénes somos y qué puede hacer cada perfil en páginas dedicadas.</p>
        </div>
        <div className="landing-acciones">
          <Link className="btn btn-landing" to="/funciones">Ver funcionalidades <i className="bi bi-arrow-right" /></Link>
          <Link className="landing-enlace" to="/nosotros">Conocer HomeClass</Link>
        </div>
      </section>
    </div>
  )
}
