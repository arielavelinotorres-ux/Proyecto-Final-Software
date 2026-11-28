import { useEffect, useState } from 'react'

// CONCEPTO 5: un HOOK PROPIO es una función cuyo nombre empieza con "use" y que
// reúne lógica reutilizable (estado + efectos). Cualquier componente puede usarla.
export default function useTheme() {
  // useState(valorInicial): devuelve [valor, funciónParaCambiarlo].
  // Pasamos una FUNCIÓN para que lea localStorage solo la primera vez.
  const [tema, setTema] = useState(() => {
    try {
      return localStorage.getItem('homeclass-tema') ?? 'dark'
    } catch {
      return 'dark' // si el navegador bloquea localStorage, seguimos sin guardar
    }
  })

  // useEffect(función, [dependencias]): ejecuta código DESPUÉS de dibujar,
  // y vuelve a ejecutarlo cada vez que cambia algo de la lista [tema].
  // Úsalo para hablar con "el mundo de afuera": el DOM, localStorage, una API...
  useEffect(() => {
    document.documentElement.setAttribute('data-theme', tema)
    try {
      localStorage.setItem('homeclass-tema', tema)
    } catch {
      /* sin almacenamiento: no pasa nada */
    }
  }, [tema])

  const alternar = () => setTema((actual) => (actual === 'dark' ? 'light' : 'dark'))
  return { tema, alternar }
}
