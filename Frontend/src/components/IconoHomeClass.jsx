const iconos = {
  marca: (
    <>
      <path d="M5 8.5c4.1-.9 7.8-.2 11 2.4v14.2c-3.1-2.5-6.8-3.3-11-2.4V8.5Z" />
      <path d="M27 8.5c-4.1-.9-7.8-.2-11 2.4v14.2c3.1-2.5 6.8-3.3 11-2.4V8.5Z" />
      <path d="M9 13.5c1.7-.2 3.3.2 4.7.9m-4.7 3c1.7-.2 3.3.2 4.7.9m4.6-3.9c1.4-.7 3-1.1 4.7-.9" />
      <path d="M16 10.9v14.2" />
    </>
  ),
  cursos: (
    <>
      <path d="M8 5.5h14a3 3 0 0 1 3 3v17H10a3 3 0 0 1-3-3v-14a3 3 0 0 1 1-2.2Z" />
      <path d="M7 21.5a3 3 0 0 1 3-3h15M12 10h8m-8 4h8" />
      <path d="M19.5 5.5v5l-1.5-1.2-1.5 1.2v-5" />
    </>
  ),
  contenido: (
    <>
      <path d="M7 8.5 22 5l3 13-15 3.5-3-13Z" />
      <path d="M9 24.5 24 21m-13-7.4 4.1 2.1 3.7-3.9" />
      <path d="m18.5 17 3.7-1.1-1.1-3.7" />
    </>
  ),
  comunicacion: (
    <>
      <path d="M5 7.5h16a3 3 0 0 1 3 3v7a3 3 0 0 1-3 3h-8l-5.5 4v-4.2A3 3 0 0 1 5 17.5v-10Z" />
      <path d="M10 12h8m-8 4h5" />
      <path d="M24 12h2a2 2 0 0 1 2 2v7l-3-2" />
    </>
  ),
  resumen: (
    <>
      <rect x="5" y="6" width="9" height="8" rx="1.5" />
      <rect x="18" y="6" width="9" height="5" rx="1.5" />
      <rect x="18" y="15" width="9" height="10" rx="1.5" />
      <path d="M8 11V9m3 2V8m10-1h3M8 21h6m-6-3h6" />
    </>
  ),
  categorias: (
    <>
      <path d="M16 8v6m-8 8v-4a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v4" />
      <circle cx="16" cy="5.5" r="2.5" />
      <circle cx="8" cy="24" r="2.5" />
      <circle cx="24" cy="24" r="2.5" />
    </>
  ),
  comunidad: (
    <>
      <circle cx="16" cy="10" r="3.5" />
      <circle cx="7.5" cy="12" r="2.5" />
      <circle cx="24.5" cy="12" r="2.5" />
      <path d="M8.5 25v-2a7.5 7.5 0 0 1 15 0v2H8.5Zm-5 0v-1.5a5 5 0 0 1 4-4.9m21 6.4v-1.5a5 5 0 0 0-4-4.9" />
    </>
  ),
  historia: (
    <>
      <path d="M8 5.5h12l4 4V26H8a2 2 0 0 1-2-2V7.5a2 2 0 0 1 2-2Z" />
      <path d="M20 5.5v5h4M11 14h10m-10 4h10m-10 4h6" />
      <path d="M4 10v14a3 3 0 0 0 3 3" />
    </>
  ),
  perfiles: (
    <>
      <circle cx="12" cy="10" r="3" />
      <circle cx="22" cy="12" r="2.5" />
      <path d="M5.5 25v-2a6.5 6.5 0 0 1 13 0v2h-13Zm14.5-7a5 5 0 0 1 7 4.6V25h-5" />
    </>
  ),
  funciones: (
    <>
      <path d="M5 7.5h9v8H5zM18 7.5h9v5h-9zM18 17.5h9v7h-9zM5 19.5h9v5H5z" />
      <path d="M9 11.5h1m12-2h1m-3 11h1m-12 1h1" />
    </>
  ),
  ayuda: (
    <>
      <path d="M6 17v-2a10 10 0 0 1 20 0v2m-20 0H5a2 2 0 0 0-2 2v2a2 2 0 0 0 2 2h3v-6H6Zm20 0h1a2 2 0 0 1 2 2v2a2 2 0 0 1-2 2h-3v-6h2Z" />
      <path d="M16 22v2.5h-4" />
    </>
  ),
}

export default function IconoHomeClass({ tipo, className = '' }) {
  const icono = iconos[tipo]
  if (!icono) throw new Error(`Icono HomeClass no definido: ${tipo}`)

  return (
    <svg
      className={`icono-homeclass ${className}`.trim()}
      viewBox="0 0 32 32"
      aria-hidden="true"
      focusable="false"
    >
      {icono}
    </svg>
  )
}
