# HomeClass · Frontend

Aplicación React + Vite. Toda lectura y escritura persistente pasa por la API ASP.NET en `Backend/HomeClassApi`; el navegador nunca se conecta directamente a SQL Server.

## Funciones conectadas

- Inicio de sesión con usuarios y sesiones almacenados en `lms_db`.
- Consulta de categorías, subcategorías, cursos y matrículas según el rol.
- Administración de cursos, categorías y subcategorías para administración.
- Consulta del contenido, foros y publicaciones; el alumnado puede registrar su progreso.
- Mensajería privada.

La interfaz solo presenta operaciones implementadas por la API activa y conectadas a los esquemas SQL. Asistencia y calificaciones no tienen tablas en los scripts actuales; las tablas de evaluaciones están incompletas y no hay rutas activas para evaluaciones, entregas o materiales. Esas pantallas antiguas no se presentan como funcionales. Consulta el [informe de errores y soluciones](docs/INFORME_ERRORES_Y_SOLUCIONES.md) para el detalle.

## Ejecutar

Requisito: Node.js 18 o superior.

```bash
cd Frontend
npm ci
npm run dev
npm run build
```

En desarrollo, Vite reenvía `/api` a `http://localhost:5000`. Para cambiarlo, copia `.env.example` como `.env` y ajusta `VITE_PROXY`. `VITE_API_URL` puede cambiar la ruta base de la API. La configuración de SQL Server solo existe en el backend.

Inicia la API desde Visual Studio siguiendo [Backend/HomeClassApi/README.md](../Backend/HomeClassApi/README.md). Debe estar conectada a la base existente `lms_db`.
