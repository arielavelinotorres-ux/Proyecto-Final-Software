using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeClassApi.Models;
using HomeClassApi.Services;

namespace HomeClassApi.Controllers
{
    [RoutePrefix("api")]
    public class LmsController : ApiController
    {
        private IHttpActionResult Error(HttpStatusCode status, string mensaje)
        {
            return ResponseMessage(Request.CreateResponse(status, new RespuestaError { Mensaje = mensaje }));
        }

        [HttpGet]
        [Route("dashboard")]
        public IHttpActionResult Dashboard()
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            var cursos = ObtenerCursos(actual);
            return Ok(new { rol = actual.Rol, usuario = actual.Nombre, cursos, totalCursos = cursos.Count });
        }

        [HttpGet]
        [Route("categorias")]
        public IHttpActionResult ListarCategorias()
        {
            if (AuthController.ObtenerUsuarioActual() == null)
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            return Ok(SqlData.ProcedureQuery("Academico.sp_ConsultarCategorias",
                SqlData.Param("@id", SqlDbType.Int, null)));
        }

        [HttpPost]
        [Route("categorias")]
        public IHttpActionResult CrearCategoria(CategoriaRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede crear categorías.");
            if (acceso != null) return acceso;
            if (request == null || string.IsNullOrWhiteSpace(request.Nombre) || request.Nombre.Trim().Length > 100)
                return Error(HttpStatusCode.BadRequest, "El nombre de la categoría es obligatorio (máximo 100 caracteres).");
            try
            {
                SqlData.ExecuteProcedure("Academico.sp_InsertarCategoria",
                    SqlData.Param("@nombre", SqlDbType.NVarChar, request.Nombre.Trim(), 100),
                    SqlData.Param("@descripcion", SqlDbType.NVarChar, TextoOpcional(request.Descripcion), 255),
                    SqlData.Param("@activo", SqlDbType.Bit, request.Activo));
                return Ok(SqlData.Query("SELECT id, nombre, descripcion, activo, creado_en FROM Academico.categorias WHERE nombre=@Nombre",
                    SqlData.Param("@Nombre", SqlDbType.NVarChar, request.Nombre.Trim(), 100)).FirstOrDefault());
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe una categoría con ese nombre.");
            }
        }

        [HttpPut]
        [Route("categorias/{id:int}")]
        public IHttpActionResult ActualizarCategoria(int id, CategoriaRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede editar categorías.");
            if (acceso != null) return acceso;
            if (request == null || id < 1 || string.IsNullOrWhiteSpace(request.Nombre) || request.Nombre.Trim().Length > 100)
                return Error(HttpStatusCode.BadRequest, "El nombre de la categoría es obligatorio (máximo 100 caracteres).");
            try
            {
                SqlData.ExecuteProcedure("Academico.sp_ActualizarCategoria",
                    SqlData.Param("@id", SqlDbType.Int, id),
                    SqlData.Param("@nombre", SqlDbType.NVarChar, request.Nombre.Trim(), 100),
                    SqlData.Param("@descripcion", SqlDbType.NVarChar, TextoOpcional(request.Descripcion), 255),
                    SqlData.Param("@activo", SqlDbType.Bit, request.Activo));
                var categoria = SqlData.Query("SELECT id, nombre, descripcion, activo, creado_en FROM Academico.categorias WHERE id=@Id",
                    SqlData.Param("@Id", SqlDbType.Int, id)).FirstOrDefault();
                return categoria == null ? Error(HttpStatusCode.NotFound, "Categoría no encontrada.") : Ok(categoria);
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe una categoría con ese nombre.");
            }
        }

        [HttpDelete]
        [Route("categorias/{id:int}")]
        public IHttpActionResult EliminarCategoria(int id)
        {
            var acceso = RequiereAdministrador("Solo administración puede eliminar categorías.");
            if (acceso != null) return acceso;
            try
            {
                if (!Existe("Academico.categorias", id)) return Error(HttpStatusCode.NotFound, "Categoría no encontrada.");
                SqlData.ExecuteProcedure("Academico.sp_EliminarCategoria", SqlData.Param("@id", SqlDbType.Int, id));
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.Conflict, "No se puede eliminar una categoría que contiene subcategorías.");
            }
        }

        [HttpGet]
        [Route("subcategorias")]
        public IHttpActionResult ListarSubcategorias()
        {
            if (AuthController.ObtenerUsuarioActual() == null)
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            var rows = SqlData.ProcedureQuery("Academico.sp_ConsultarSubcategorias",
                SqlData.Param("@id", SqlDbType.Int, null));
            foreach (var row in rows)
            {
                var categoria = SqlData.Query("SELECT nombre FROM Academico.categorias WHERE id=@Id",
                    SqlData.Param("@Id", SqlDbType.Int, row["categoria_id"])).FirstOrDefault();
                row["categoria"] = categoria == null ? null : categoria["nombre"];
            }
            return Ok(rows);
        }

        [HttpPost]
        [Route("subcategorias")]
        public IHttpActionResult CrearSubcategoria(SubcategoriaRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede crear subcategorías.");
            if (acceso != null) return acceso;
            if (request == null || request.CategoriaId < 1 || string.IsNullOrWhiteSpace(request.Nombre) || request.Nombre.Trim().Length > 100)
                return Error(HttpStatusCode.BadRequest, "Selecciona una categoría e ingresa un nombre (máximo 100 caracteres).");
            try
            {
                SqlData.ExecuteProcedure("Academico.sp_InsertarSubcategoria",
                    SqlData.Param("@categoria_id", SqlDbType.Int, request.CategoriaId),
                    SqlData.Param("@nombre", SqlDbType.NVarChar, request.Nombre.Trim(), 100),
                    SqlData.Param("@descripcion", SqlDbType.NVarChar, TextoOpcional(request.Descripcion), 255),
                    SqlData.Param("@activo", SqlDbType.Bit, request.Activo));
                return Ok(SqlData.Query("SELECT TOP (1) id, categoria_id, nombre, descripcion, activo, creado_en FROM Academico.subcategorias WHERE categoria_id=@CategoriaId AND nombre=@Nombre ORDER BY id DESC",
                    SqlData.Param("@CategoriaId", SqlDbType.Int, request.CategoriaId),
                    SqlData.Param("@Nombre", SqlDbType.NVarChar, request.Nombre.Trim(), 100)).FirstOrDefault());
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.BadRequest, "La categoría seleccionada no existe.");
            }
        }

        [HttpPut]
        [Route("subcategorias/{id:int}")]
        public IHttpActionResult ActualizarSubcategoria(int id, SubcategoriaRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede editar subcategorías.");
            if (acceso != null) return acceso;
            if (request == null || id < 1 || request.CategoriaId < 1 || string.IsNullOrWhiteSpace(request.Nombre) || request.Nombre.Trim().Length > 100)
                return Error(HttpStatusCode.BadRequest, "Selecciona una categoría e ingresa un nombre (máximo 100 caracteres).");
            if (!Existe("Academico.subcategorias", id)) return Error(HttpStatusCode.NotFound, "Subcategoría no encontrada.");
            try
            {
                SqlData.ExecuteProcedure("Academico.sp_ActualizarSubcategoria",
                    SqlData.Param("@id", SqlDbType.Int, id),
                    SqlData.Param("@categoria_id", SqlDbType.Int, request.CategoriaId),
                    SqlData.Param("@nombre", SqlDbType.NVarChar, request.Nombre.Trim(), 100),
                    SqlData.Param("@descripcion", SqlDbType.NVarChar, TextoOpcional(request.Descripcion), 255),
                    SqlData.Param("@activo", SqlDbType.Bit, request.Activo));
                return Ok(SqlData.Query("SELECT id, categoria_id, nombre, descripcion, activo, creado_en FROM Academico.subcategorias WHERE id=@Id",
                    SqlData.Param("@Id", SqlDbType.Int, id)).FirstOrDefault());
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.BadRequest, "La categoría seleccionada no existe.");
            }
        }

        [HttpDelete]
        [Route("subcategorias/{id:int}")]
        public IHttpActionResult EliminarSubcategoria(int id)
        {
            var acceso = RequiereAdministrador("Solo administración puede eliminar subcategorías.");
            if (acceso != null) return acceso;
            try
            {
                if (!Existe("Academico.subcategorias", id)) return Error(HttpStatusCode.NotFound, "Subcategoría no encontrada.");
                SqlData.ExecuteProcedure("Academico.sp_EliminarSubcategoria", SqlData.Param("@id", SqlDbType.Int, id));
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.Conflict, "No se puede eliminar una subcategoría asignada a un curso.");
            }
        }

        [HttpGet]
        [Route("usuarios")]
        public IHttpActionResult ListarUsuarios(string rol = null)
        {
            var acceso = RequiereAdministrador("Solo administración puede consultar usuarios.");
            if (acceso != null) return acceso;
            var rows = SqlData.Query(
                "SELECT u.id AS id,u.correo AS correo,CAST(COALESCE(u.activo,1) AS BIT) AS activo," +
                "CASE LOWER(r.nombre) WHEN N'administrador' THEN N'admin' WHEN N'docente' THEN N'docente' WHEN N'estudiante' THEN N'estudiante' END AS rol," +
                "COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(e.nombre,N' ',e.apellido))),N''),NULLIF(LTRIM(RTRIM(CONCAT(d.nombre,N' ',d.apellido))),N''),u.correo) AS nombre " +
                "FROM Usuarios.usuarios u JOIN Usuarios.roles r ON r.id=u.rol_id " +
                "LEFT JOIN Usuarios.estudiantes e ON e.usuario_id=u.id LEFT JOIN Usuarios.docentes d ON d.usuario_id=u.id " +
                "WHERE COALESCE(u.activo,1)=1 ORDER BY nombre");
            if (!string.IsNullOrWhiteSpace(rol))
                rows = rows.Where(x => string.Equals(Convert.ToString(x["rol"]), rol, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(rows);
        }

        [HttpGet]
        [Route("contactos")]
        public IHttpActionResult ListarContactos()
        {
            if (AuthController.ObtenerUsuarioActual() == null)
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            return Ok(SqlData.Query(
                "SELECT u.id AS id," +
                "CASE LOWER(r.nombre) WHEN N'administrador' THEN N'admin' WHEN N'docente' THEN N'docente' WHEN N'estudiante' THEN N'estudiante' END AS rol," +
                "COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(e.nombre,N' ',e.apellido))),N''),NULLIF(LTRIM(RTRIM(CONCAT(d.nombre,N' ',d.apellido))),N''),u.correo) AS nombre " +
                "FROM Usuarios.usuarios u JOIN Usuarios.roles r ON r.id=u.rol_id " +
                "LEFT JOIN Usuarios.estudiantes e ON e.usuario_id=u.id LEFT JOIN Usuarios.docentes d ON d.usuario_id=u.id " +
                "WHERE COALESCE(u.activo,1)=1 ORDER BY nombre"));
        }

        [HttpGet]
        [Route("cursos")]
        public IHttpActionResult ListarCursos()
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            return Ok(ObtenerCursos(actual));
        }

        [HttpGet]
        [Route("cursos/{id:int}")]
        public IHttpActionResult ObtenerCurso(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            var curso = ResumenCurso(id);
            if (curso == null || !PuedeAcceder(actual, id))
                return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            curso["estudiantes"] = actual.Rol == "estudiante"
                ? new List<Dictionary<string, object>>()
                : EstudiantesMatriculados(id, actual.Rol == "admin");
            return Ok(curso);
        }

        [HttpPost]
        [Route("cursos")]
        public IHttpActionResult CrearCurso(CursoRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede crear cursos.");
            if (acceso != null) return acceso;
            var error = ValidarCurso(request);
            if (error != null) return Error(HttpStatusCode.BadRequest, error);
            var docenteId = PerfilDocente(request.DocenteId.Value);
            if (!docenteId.HasValue) return Error(HttpStatusCode.BadRequest, "Selecciona un docente registrado.");
            try
            {
                SqlData.ExecuteProcedure("Academico.sp_InsertarCurso", ParametrosCurso(request, docenteId.Value));
                var creado = SqlData.Query("SELECT id FROM Academico.cursos WHERE codigo_curso=@Codigo",
                    SqlData.Param("@Codigo", SqlDbType.NVarChar, request.Codigo.Trim(), 20)).FirstOrDefault();
                if (creado == null) return Error(HttpStatusCode.InternalServerError, "No se pudo confirmar la creación del curso.");
                return Ok(ResumenCurso(Convert.ToInt32(creado["id"])));
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe un curso con ese código.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.BadRequest, "La subcategoría seleccionada no existe.");
            }
        }

        [HttpPut]
        [Route("cursos/{id:int}")]
        public IHttpActionResult ActualizarCurso(int id, CursoRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede editar cursos.");
            if (acceso != null) return acceso;
            var error = ValidarCurso(request);
            if (error != null) return Error(HttpStatusCode.BadRequest, error);
            var docenteId = PerfilDocente(request.DocenteId.Value);
            if (!docenteId.HasValue) return Error(HttpStatusCode.BadRequest, "Selecciona un docente registrado.");
            if (ResumenCurso(id) == null) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            try
            {
                var parametros = ParametrosCurso(request, docenteId.Value).Concat(new[] { SqlData.Param("@id", SqlDbType.Int, id) }).ToArray();
                SqlData.ExecuteProcedure("Academico.sp_ActualizarCurso", parametros);
                return Ok(ResumenCurso(id));
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe un curso con ese código.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.BadRequest, "La subcategoría seleccionada no existe.");
            }
        }

        [HttpDelete]
        [Route("cursos/{id:int}")]
        public IHttpActionResult EliminarCurso(int id)
        {
            var acceso = RequiereAdministrador("Solo administración puede eliminar cursos.");
            if (acceso != null) return acceso;
            if (ResumenCurso(id) == null) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            try
            {
                SqlData.ExecuteProcedure("Academico.sp_EliminarCurso", SqlData.Param("@id", SqlDbType.Int, id));
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.Conflict, "No se puede eliminar un curso que tiene matrículas, foros o contenido asociado.");
            }
        }

        [HttpPost]
        [Route("cursos/{id:int}/estudiantes")]
        public IHttpActionResult Matricular(int id, MatriculaRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede matricular estudiantes.");
            if (acceso != null) return acceso;
            if (request == null || ResumenCurso(id) == null) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            var estudianteId = PerfilEstudiante(request.EstudianteId);
            if (!estudianteId.HasValue) return Error(HttpStatusCode.BadRequest, "Selecciona un estudiante activo.");
            try
            {
                SqlData.ExecuteProcedure("Academico.sp_InsertarMatricula",
                    SqlData.Param("@estudiante_id", SqlDbType.Int, estudianteId.Value),
                    SqlData.Param("@curso_id", SqlDbType.Int, id),
                    SqlData.Param("@estado", SqlDbType.NVarChar, "Activo", 20));
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "El estudiante ya está matriculado en este curso.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.BadRequest, "El curso o el estudiante ya no está disponible.");
            }
        }

        [HttpDelete]
        [Route("cursos/{id:int}/estudiantes/{estudianteId:int}")]
        public IHttpActionResult Desmatricular(int id, int estudianteId)
        {
            var acceso = RequiereAdministrador("Solo administración puede modificar matrículas.");
            if (acceso != null) return acceso;
            var perfilId = PerfilEstudiante(estudianteId);
            if (!perfilId.HasValue) return Error(HttpStatusCode.NotFound, "Estudiante no encontrado.");
            var matricula = SqlData.ProcedureQuery("Academico.sp_ConsultarMatriculas",
                SqlData.Param("@id", SqlDbType.Int, null))
                .FirstOrDefault(row => Convert.ToInt32(row["curso_id"]) == id &&
                    Convert.ToInt32(row["estudiante_id"]) == perfilId.Value);
            if (matricula == null) return Error(HttpStatusCode.NotFound, "Matrícula no encontrada.");
            SqlData.ExecuteProcedure("Academico.sp_EliminarMatricula",
                SqlData.Param("@id", SqlDbType.Int, matricula["id"]));
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [Route("cursos/{id:int}/estudiantes/{estudianteId:int}")]
        public IHttpActionResult ActualizarMatricula(int id, int estudianteId, EstadoMatriculaRequest request)
        {
            var acceso = RequiereAdministrador("Solo administración puede modificar matrículas.");
            if (acceso != null) return acceso;
            if (request == null || !new[] { "Activo", "Completado", "Cancelado" }.Contains(request.Estado))
                return Error(HttpStatusCode.BadRequest, "El estado debe ser Activo, Completado o Cancelado.");
            var perfilId = PerfilEstudiante(estudianteId);
            if (!perfilId.HasValue) return Error(HttpStatusCode.NotFound, "Estudiante no encontrado.");
            var matricula = SqlData.ProcedureQuery("Academico.sp_ConsultarMatriculas",
                SqlData.Param("@id", SqlDbType.Int, null))
                .FirstOrDefault(row => Convert.ToInt32(row["curso_id"]) == id &&
                    Convert.ToInt32(row["estudiante_id"]) == perfilId.Value);
            if (matricula == null) return Error(HttpStatusCode.NotFound, "Matrícula no encontrada.");
            SqlData.ExecuteProcedure("Academico.sp_ActualizarMatricula",
                SqlData.Param("@id", SqlDbType.Int, matricula["id"]),
                SqlData.Param("@estudiante_id", SqlDbType.Int, perfilId.Value),
                SqlData.Param("@curso_id", SqlDbType.Int, id),
                SqlData.Param("@estado", SqlDbType.NVarChar, request.Estado, 20));
            return Ok(new { cursoId = id, estudianteId, estado = request.Estado });
        }

        [HttpGet]
        [Route("cursos/{id:int}/contenido")]
        public IHttpActionResult ContenidoCurso(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!PuedeAcceder(actual, id)) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            var secciones = SqlData.Query(
                "SELECT id, curso_id AS cursoId, titulo, orden FROM Contenido.secciones WHERE curso_id=@CursoId ORDER BY orden,id",
                SqlData.Param("@CursoId", SqlDbType.Int, id));
            foreach (var seccion in secciones)
            {
                seccion["lecciones"] = SqlData.Query(
                    "SELECT l.id,l.titulo,l.contenido_texto AS contenido,l.url_video AS urlVideo,l.duracion_minutos AS duracionMinutos,l.orden,l.es_gratuita AS esGratuita " +
                    "FROM Contenido.lecciones l WHERE l.seccion_id=@SeccionId ORDER BY l.orden,l.id",
                    SqlData.Param("@SeccionId", SqlDbType.Int, seccion["id"]));
                var lecciones = (List<Dictionary<string, object>>)seccion["lecciones"];
                foreach (var leccion in lecciones)
                {
                    leccion["recursos"] = SqlData.Query(
                        "SELECT id,nombre,tipo_archivo AS tipoArchivo,url_recurso AS urlRecurso FROM Contenido.recursos WHERE leccion_id=@LeccionId ORDER BY id",
                        SqlData.Param("@LeccionId", SqlDbType.Int, leccion["id"]));
                    if (actual.Rol == "estudiante")
                    {
                        var estudianteId = PerfilEstudiante(actual.Id);
                        var progreso = estudianteId.HasValue
                            ? SqlData.Scalar("SELECT completado FROM Monitoreo.progreso_lecciones WHERE estudiante_id=@EstudianteId AND leccion_id=@LeccionId",
                                SqlData.Param("@EstudianteId", SqlDbType.Int, estudianteId.Value),
                                SqlData.Param("@LeccionId", SqlDbType.Int, leccion["id"]))
                            : null;
                        leccion["completado"] = progreso != null && progreso != DBNull.Value && Convert.ToBoolean(progreso);
                    }
                }
            }
            return Ok(secciones);
        }

        [HttpPut]
        [Route("lecciones/{id:int}/progreso")]
        public IHttpActionResult ActualizarProgreso(int id, ProgresoLeccionRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (actual.Rol != "estudiante") return Error(HttpStatusCode.Forbidden, "Solo estudiantes pueden actualizar su progreso.");
            if (request == null) return Error(HttpStatusCode.BadRequest, "Indica el estado de la lección.");
            var estudianteId = PerfilEstudiante(actual.Id);
            var cursoId = Convert.ToInt32(SqlData.Scalar(
                "SELECT s.curso_id FROM Contenido.lecciones l JOIN Contenido.secciones s ON s.id=l.seccion_id WHERE l.id=@LeccionId",
                SqlData.Param("@LeccionId", SqlDbType.Int, id)) ?? 0);
            if (!estudianteId.HasValue || cursoId == 0 || !PuedeAcceder(actual, cursoId))
                return Error(HttpStatusCode.NotFound, "Lección no encontrada.");
            SqlData.Execute(
                "IF EXISTS (SELECT 1 FROM Monitoreo.progreso_lecciones WHERE estudiante_id=@EstudianteId AND leccion_id=@LeccionId) " +
                "UPDATE Monitoreo.progreso_lecciones SET completado=@Completado, fecha_completado=CASE WHEN @Completado=1 THEN COALESCE(fecha_completado,GETDATE()) ELSE NULL END WHERE estudiante_id=@EstudianteId AND leccion_id=@LeccionId " +
                "ELSE INSERT Monitoreo.progreso_lecciones (estudiante_id,leccion_id,completado,fecha_completado) VALUES (@EstudianteId,@LeccionId,@Completado,CASE WHEN @Completado=1 THEN GETDATE() ELSE NULL END)",
                SqlData.Param("@EstudianteId", SqlDbType.Int, estudianteId.Value),
                SqlData.Param("@LeccionId", SqlDbType.Int, id),
                SqlData.Param("@Completado", SqlDbType.Bit, request.Completado));
            return Ok(new { leccionId = id, completado = request.Completado });
        }

        [HttpGet]
        [Route("cursos/{id:int}/foros")]
        public IHttpActionResult ListarForos(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!PuedeAcceder(actual, id)) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            var foros = SqlData.ProcedureQuery("Comunicacion.SP_ListarForos",
                SqlData.Param("@curso_id", SqlDbType.Int, id));
            foreach (var foro in foros)
            {
                foro["publicaciones"] = SqlData.Scalar("SELECT COUNT(*) FROM Comunicacion.publicaciones_foro WHERE foro_id=@ForoId",
                    SqlData.Param("@ForoId", SqlDbType.Int, foro["id"]));
            }
            return Ok(foros);
        }

        [HttpPost]
        [Route("cursos/{id:int}/foros")]
        public IHttpActionResult CrearForo(int id, AvisoRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (actual.Rol != "docente" || !PuedeAcceder(actual, id))
                return Error(HttpStatusCode.Forbidden, "Solo el docente del curso puede crear foros.");
            if (request == null || string.IsNullOrWhiteSpace(request.Titulo) || request.Titulo.Trim().Length > 150 ||
                string.IsNullOrWhiteSpace(request.Mensaje) || request.Mensaje.Trim().Length > 100)
                return Error(HttpStatusCode.BadRequest, "El título y la descripción son obligatorios (máximo 150 y 100 caracteres).");
            var idCreado = SqlData.ProcedureScalar("Comunicacion.SP_InsertarForo",
                SqlData.Param("@curso_id", SqlDbType.Int, id),
                SqlData.Param("@titulo", SqlDbType.NVarChar, request.Titulo.Trim(), 150),
                SqlData.Param("@descripcion", SqlDbType.NVarChar, request.Mensaje.Trim(), 100));
            if (idCreado == null || idCreado == DBNull.Value)
                return Error(HttpStatusCode.InternalServerError, "No se pudo confirmar la creación del foro.");
            return Ok(new { id = Convert.ToInt32(idCreado), curso_id = id, titulo = request.Titulo.Trim(), descripcion = request.Mensaje.Trim(), estado = "Abierto" });
        }

        [HttpPut]
        [Route("foros/{id:int}")]
        public IHttpActionResult ActualizarForo(int id, ForoRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (actual.Rol != "docente" || !PuedeAccederForo(actual, id))
                return Error(HttpStatusCode.Forbidden, "Solo el docente del curso puede editar el foro.");
            if (request == null || string.IsNullOrWhiteSpace(request.Titulo) || request.Titulo.Trim().Length > 150 ||
                string.IsNullOrWhiteSpace(request.Descripcion) || request.Descripcion.Trim().Length > 100 ||
                !new[] { "Abierto", "Cerrado", "Archivado" }.Contains(request.Estado))
                return Error(HttpStatusCode.BadRequest, "El título, la descripción (máximo 100 caracteres) y el estado del foro son obligatorios.");
            SqlData.ExecuteProcedure("Comunicacion.SP_ActualizarForo",
                SqlData.Param("@id", SqlDbType.Int, id),
                SqlData.Param("@titulo", SqlDbType.NVarChar, request.Titulo.Trim(), 150),
                SqlData.Param("@descripcion", SqlDbType.NVarChar, request.Descripcion.Trim(), 100),
                SqlData.Param("@estado", SqlDbType.NVarChar, request.Estado, 20));
            return Ok(new { id, titulo = request.Titulo.Trim(), descripcion = request.Descripcion.Trim(), estado = request.Estado });
        }

        [HttpDelete]
        [Route("foros/{id:int}")]
        public IHttpActionResult ArchivarForo(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (actual.Rol != "docente" || !PuedeAccederForo(actual, id))
                return Error(HttpStatusCode.Forbidden, "Solo el docente del curso puede archivar el foro.");
            SqlData.ExecuteProcedure("Comunicacion.SP_DesactivarForo", SqlData.Param("@id", SqlDbType.Int, id));
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("foros/{id:int}/publicaciones")]
        public IHttpActionResult ListarPublicaciones(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!PuedeAccederForo(actual, id)) return Error(HttpStatusCode.NotFound, "Foro no encontrado.");
            return Ok(SqlData.Query(
                "SELECT p.id,p.foro_id AS foroId,p.usuario_id AS usuarioId,p.publicacion_padre_id AS publicacionPadreId,p.contenido,p.creado_en AS creadoEn," +
                "COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(e.nombre,N' ',e.apellido))),N''),NULLIF(LTRIM(RTRIM(CONCAT(d.nombre,N' ',d.apellido))),N''),u.correo) AS autor " +
                "FROM Comunicacion.publicaciones_foro p JOIN Usuarios.usuarios u ON u.id=p.usuario_id " +
                "LEFT JOIN Usuarios.estudiantes e ON e.usuario_id=u.id LEFT JOIN Usuarios.docentes d ON d.usuario_id=u.id " +
                "WHERE p.foro_id=@ForoId ORDER BY p.creado_en,p.id",
                SqlData.Param("@ForoId", SqlDbType.Int, id)));
        }

        [HttpPost]
        [Route("foros/{id:int}/publicaciones")]
        public IHttpActionResult CrearPublicacion(int id, PublicacionForoRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!PuedeAccederForo(actual, id)) return Error(HttpStatusCode.NotFound, "Foro no encontrado.");
            var estadoForo = Convert.ToString(SqlData.Scalar("SELECT estado FROM Comunicacion.foros WHERE id=@Id",
                SqlData.Param("@Id", SqlDbType.Int, id)));
            if (estadoForo != "Abierto") return Error(HttpStatusCode.Conflict, "Este foro está cerrado o archivado.");
            if (request == null || string.IsNullOrWhiteSpace(request.Contenido))
                return Error(HttpStatusCode.BadRequest, "El contenido de la publicación es obligatorio.");
            if (request.PublicacionPadreId.HasValue && Convert.ToInt32(SqlData.Scalar(
                "SELECT COUNT(*) FROM Comunicacion.publicaciones_foro WHERE id=@Id AND foro_id=@ForoId",
                SqlData.Param("@Id", SqlDbType.Int, request.PublicacionPadreId.Value),
                SqlData.Param("@ForoId", SqlDbType.Int, id))) == 0)
                return Error(HttpStatusCode.BadRequest, "La publicación a responder no pertenece a este foro.");
            var newId = Convert.ToInt32(SqlData.Scalar(
                "INSERT Comunicacion.publicaciones_foro (foro_id,usuario_id,publicacion_padre_id,contenido) OUTPUT INSERTED.id VALUES (@ForoId,@UsuarioId,@PadreId,@Contenido)",
                SqlData.Param("@ForoId", SqlDbType.Int, id),
                SqlData.Param("@UsuarioId", SqlDbType.Int, actual.Id),
                SqlData.Param("@PadreId", SqlDbType.Int, request.PublicacionPadreId),
                SqlData.Param("@Contenido", SqlDbType.NVarChar, request.Contenido.Trim(), -1)));
            return Ok(new { id = newId });
        }

        [HttpGet]
        [Route("mensajes")]
        public IHttpActionResult ListarMensajes()
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            var mensajes = SqlData.Query(
                "SELECT id,emisor_id,receptor_id,asunto,contenido,leido,fecha_envio FROM Comunicacion.mensajes_privados " +
                "WHERE receptor_id=@ReceptorId ORDER BY fecha_envio DESC",
                SqlData.Param("@ReceptorId", SqlDbType.Int, actual.Id));
            foreach (var mensaje in mensajes)
            {
                var remitente = SqlData.Query(
                    "SELECT COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(e.nombre,N' ',e.apellido))),N''),NULLIF(LTRIM(RTRIM(CONCAT(d.nombre,N' ',d.apellido))),N''),u.correo) AS nombre " +
                    "FROM Usuarios.usuarios u LEFT JOIN Usuarios.estudiantes e ON e.usuario_id=u.id LEFT JOIN Usuarios.docentes d ON d.usuario_id=u.id WHERE u.id=@Id",
                    SqlData.Param("@Id", SqlDbType.Int, mensaje["emisor_id"])).FirstOrDefault();
                mensaje["remitente"] = remitente == null ? null : remitente["nombre"];
            }
            return Ok(mensajes);
        }

        [HttpPut]
        [Route("mensajes/{id:int}/leido")]
        public IHttpActionResult MarcarMensajeLeido(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            var mensaje = SqlData.Query(
                "SELECT id,asunto,contenido FROM Comunicacion.mensajes_privados WHERE id=@Id AND receptor_id=@UsuarioId",
                SqlData.Param("@Id", SqlDbType.Int, id),
                SqlData.Param("@UsuarioId", SqlDbType.Int, actual.Id)).FirstOrDefault();
            if (mensaje == null) return Error(HttpStatusCode.NotFound, "Mensaje no encontrado.");
            if (Convert.ToString(mensaje["contenido"]).Length > 200)
                return Error(HttpStatusCode.Conflict, "La base de datos solo permite actualizar mensajes de hasta 200 caracteres.");
            SqlData.ExecuteProcedure("Comunicacion.SP_ActualizarMensajePrivado",
                SqlData.Param("@id", SqlDbType.Int, id),
                SqlData.Param("@asunto", SqlDbType.NVarChar, TextoOpcional(Convert.ToString(mensaje["asunto"])), 150),
                SqlData.Param("@contenido", SqlDbType.NVarChar, Convert.ToString(mensaje["contenido"]), 200),
                SqlData.Param("@leido", SqlDbType.Bit, true));
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("mensajes")]
        public IHttpActionResult EnviarMensaje(MensajePrivadoRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (request == null || request.ReceptorId < 1 || request.ReceptorId == actual.Id ||
                string.IsNullOrWhiteSpace(request.Contenido) || request.Contenido.Trim().Length > 200 ||
                (request.Asunto ?? string.Empty).Trim().Length > 150)
                return Error(HttpStatusCode.BadRequest, "El destinatario debe ser válido y el mensaje debe tener entre 1 y 200 caracteres.");
            if (Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM Usuarios.usuarios WHERE id=@Id AND COALESCE(activo,1)=1",
                SqlData.Param("@Id", SqlDbType.Int, request.ReceptorId))) == 0)
                return Error(HttpStatusCode.BadRequest, "El destinatario no existe o está inactivo.");
            try
            {
                var id = SqlData.Scalar(
                    "INSERT Comunicacion.mensajes_privados (emisor_id,receptor_id,asunto,contenido) OUTPUT INSERTED.id VALUES (@EmisorId,@ReceptorId,@Asunto,@Contenido)",
                    SqlData.Param("@EmisorId", SqlDbType.Int, actual.Id),
                    SqlData.Param("@ReceptorId", SqlDbType.Int, request.ReceptorId),
                    SqlData.Param("@Asunto", SqlDbType.NVarChar, TextoOpcional(request.Asunto), 150),
                    SqlData.Param("@Contenido", SqlDbType.NVarChar, request.Contenido.Trim(), 200));
                return Ok(new { id = Convert.ToInt32(id) });
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error(HttpStatusCode.BadRequest, "El destinatario no existe.");
            }
        }

        private List<Dictionary<string, object>> ObtenerCursos(UsuarioRespuesta actual)
        {
            var rows = SqlData.ProcedureQuery("Academico.sp_ConsultarCursos",
                SqlData.Param("@id", SqlDbType.Int, null));
            return rows.Where(row => PuedeAcceder(actual, Convert.ToInt32(row["id"])))
                .Select(row => ResumenCurso(Convert.ToInt32(row["id"])))
                .Where(row => row != null)
                .ToList();
        }

        private static Dictionary<string, object> ResumenCurso(int id)
        {
            return SqlData.Query(
                "SELECT c.id,c.codigo_curso AS codigo,c.titulo AS nombre,c.descripcion,c.estado,c.imagen_portada AS imagenPortada," +
                "c.subcategoria_id AS subcategoriaId,s.nombre AS subcategoria,s.categoria_id AS categoriaId,ca.nombre AS categoria," +
                "d.usuario_id AS docenteId,COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(d.nombre,N' ',d.apellido))),N''),du.correo) AS docente," +
                "(SELECT COUNT(*) FROM Academico.matriculas m WHERE m.curso_id=c.id AND m.estado=N'Activo') AS estudiantes " +
                "FROM Academico.cursos c JOIN Academico.subcategorias s ON s.id=c.subcategoria_id " +
                "JOIN Academico.categorias ca ON ca.id=s.categoria_id JOIN Usuarios.docentes d ON d.id=c.docente_id " +
                "JOIN Usuarios.usuarios du ON du.id=d.usuario_id " +
                "WHERE c.id=@Id",
                SqlData.Param("@Id", SqlDbType.Int, id)).FirstOrDefault();
        }

        private List<Dictionary<string, object>> EstudiantesMatriculados(int cursoId, bool incluirInactivos)
        {
            var matriculas = SqlData.ProcedureQuery("Academico.sp_ConsultarMatriculas",
                SqlData.Param("@id", SqlDbType.Int, null))
                .Where(row => Convert.ToInt32(row["curso_id"]) == cursoId &&
                    (incluirInactivos || Convert.ToString(row["estado"]) == "Activo"));
            return matriculas.Select(row =>
            {
                var student = SqlData.Query(
                    "SELECT u.id AS id,u.correo AS correo," +
                    "COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(e.nombre,N' ',e.apellido))),N''),u.correo) AS nombre " +
                    "FROM Usuarios.estudiantes e JOIN Usuarios.usuarios u ON u.id=e.usuario_id WHERE e.id=@Id",
                    SqlData.Param("@Id", SqlDbType.Int, row["estudiante_id"])).FirstOrDefault();
                if (student != null) student["estado"] = row["estado"];
                return student;
            }).Where(row => row != null).ToList();
        }

        private bool PuedeAcceder(UsuarioRespuesta actual, int cursoId)
        {
            if (actual.Rol == "admin") return Existe("Academico.cursos", cursoId);
            var perfil = actual.Rol == "docente" ? PerfilDocente(actual.Id) : PerfilEstudiante(actual.Id);
            if (!perfil.HasValue) return false;
            var tabla = actual.Rol == "docente" ? "Academico.cursos" : "Academico.matriculas";
            var filtro = actual.Rol == "docente"
                ? "id=@CursoId AND docente_id=@PerfilId"
                : "curso_id=@CursoId AND estudiante_id=@PerfilId AND estado=N'Activo'";
            return Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM " + tabla + " WHERE " + filtro,
                SqlData.Param("@CursoId", SqlDbType.Int, cursoId),
                SqlData.Param("@PerfilId", SqlDbType.Int, perfil.Value))) > 0;
        }

        private bool PuedeAccederForo(UsuarioRespuesta actual, int foroId)
        {
            var curso = SqlData.Scalar("SELECT curso_id FROM Comunicacion.foros WHERE id=@Id",
                SqlData.Param("@Id", SqlDbType.Int, foroId));
            return curso != null && curso != DBNull.Value && PuedeAcceder(actual, Convert.ToInt32(curso));
        }

        private static int? PerfilDocente(int usuarioId)
        {
            var value = SqlData.Scalar("SELECT d.id FROM Usuarios.docentes d JOIN Usuarios.usuarios u ON u.id=d.usuario_id WHERE d.usuario_id=@Id AND COALESCE(u.activo,1)=1",
                SqlData.Param("@Id", SqlDbType.Int, usuarioId));
            return value == null || value == DBNull.Value ? (int?)null : Convert.ToInt32(value);
        }

        private static int? PerfilEstudiante(int usuarioId)
        {
            var value = SqlData.Scalar("SELECT e.id FROM Usuarios.estudiantes e JOIN Usuarios.usuarios u ON u.id=e.usuario_id WHERE e.usuario_id=@Id AND COALESCE(u.activo,1)=1",
                SqlData.Param("@Id", SqlDbType.Int, usuarioId));
            return value == null || value == DBNull.Value ? (int?)null : Convert.ToInt32(value);
        }

        private IHttpActionResult RequiereAdministrador(string mensaje)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            return actual.Rol == "admin" ? null : Error(HttpStatusCode.Forbidden, mensaje);
        }

        private static bool Existe(string tabla, int id)
        {
            return Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM " + tabla + " WHERE id=@Id",
                SqlData.Param("@Id", SqlDbType.Int, id))) > 0;
        }

        private static string ValidarCurso(CursoRequest curso)
        {
            if (curso == null || string.IsNullOrWhiteSpace(curso.Codigo) || curso.Codigo.Trim().Length > 20 ||
                string.IsNullOrWhiteSpace(curso.Nombre) || curso.Nombre.Trim().Length > 150)
                return "El código y el nombre del curso son obligatorios (máximo 20 y 150 caracteres).";
            if (!curso.DocenteId.HasValue || !curso.SubcategoriaId.HasValue || curso.SubcategoriaId.Value < 1)
                return "Selecciona el docente y la subcategoría del curso.";
            if ((curso.ImagenPortada ?? string.Empty).Length > 255)
                return "La ruta de portada no puede superar 255 caracteres.";
            var estado = string.IsNullOrWhiteSpace(curso.Estado) ? "Borrador" : curso.Estado.Trim();
            if (!new[] { "Borrador", "Publicado", "Archivado" }.Contains(estado))
                return "El estado del curso no es válido.";
            return null;
        }

        private static SqlParameter[] ParametrosCurso(CursoRequest curso, int docentePerfilId)
        {
            var estado = string.IsNullOrWhiteSpace(curso.Estado) ? "Borrador" : curso.Estado.Trim();
            return new[]
            {
                SqlData.Param("@subcategoria_id", SqlDbType.Int, curso.SubcategoriaId.Value),
                SqlData.Param("@docente_id", SqlDbType.Int, docentePerfilId),
                SqlData.Param("@titulo", SqlDbType.NVarChar, curso.Nombre.Trim(), 150),
                SqlData.Param("@descripcion", SqlDbType.NVarChar, TextoOpcional(curso.Descripcion), -1),
                SqlData.Param("@codigo_curso", SqlDbType.NVarChar, curso.Codigo.Trim(), 20),
                SqlData.Param("@imagen_portada", SqlDbType.NVarChar, TextoOpcional(curso.ImagenPortada), 255),
                SqlData.Param("@estado", SqlDbType.NVarChar, estado, 20)
            };
        }

        private static string TextoOpcional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
