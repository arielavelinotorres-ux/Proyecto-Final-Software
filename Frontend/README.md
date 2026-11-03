# HomeClass · Frontend

Aplicación React + Vite. Toda lectura y escritura persistente pasa por la API ASP.NET en `Backend/HomeClassApi`; el navegador nunca se conecta directamente a SQL Server.

## Funciones conectadas

- Inicio de sesión con usuarios y sesiones almacenados en `lms_db`.
- Consulta de categorías, subcategorías, cursos y matrículas.
- Administración de cursos y catálogo según el rol.
- Consulta del contenido del curso, foros, publicaciones y mensajes privados.

La interfaz solo muestra operaciones que están representadas por los esquemas y procedimientos SQL existentes. Los scripts de esta base no contienen tablas para asistencia, calificaciones ni una entidad de evaluación completa, por lo que esas pantallas no se presentan como funcionales.

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
