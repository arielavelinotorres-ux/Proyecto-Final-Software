using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeClassApi.Controllers;
using HomeClassApi.Models;
using HomeClassApi.Services;

namespace HomeClassApi.Controllers
{
    [RoutePrefix("api/cursos")]
    public class CursosController : ApiController
    {
        private IHttpActionResult Error(HttpStatusCode status, string mensaje)
        {
            return ResponseMessage(Request.CreateResponse(status, new RespuestaError { Mensaje = mensaje }));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Listar()
        {
            var usuarioActual = AuthController.ObtenerUsuarioActual();
            if (usuarioActual == null)
            {
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            }
            if (usuarioActual.Rol == "admin" && !AuthService.TienePermisoAdmin(usuarioActual, "cursos") && !AuthService.TienePermisoAdmin(usuarioActual, "reportes"))
                return Error(HttpStatusCode.Forbidden, "No tienes permiso para consultar cursos.");

            return Ok(AuthService.ObtenerCursos(usuarioActual.Rol, usuarioActual.Id));
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult Obtener(int id)
        {
            var usuarioActual = AuthController.ObtenerUsuarioActual();
            if (usuarioActual == null)
            {
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            }
            if (usuarioActual.Rol == "admin" && !AuthService.TienePermisoAdmin(usuarioActual, "cursos"))
                return Error(HttpStatusCode.Forbidden, "No tienes permiso para consultar cursos.");

            if (!TieneAcceso(usuarioActual.Rol, usuarioActual.Id, id))
            {
                return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            }

            var curso = SqlData.Query(
                "SELECT c.Id AS id, c.Codigo AS codigo, c.Nombre AS nombre, c.Periodo AS periodo, c.Descripcion AS descripcion, c.DocenteId AS docenteId, COALESCE(u.Nombre, N'Sin docente') AS docente " +
                "FROM dbo.Curso c LEFT JOIN dbo.Usuario u ON u.Id = c.DocenteId WHERE c.Id = @Id",
                SqlData.Param("@Id", SqlDbType.Int, id)).FirstOrDefault();
            if (curso == null) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            curso["estudiantes"] = usuarioActual.Rol == "estudiante"
                ? (object)new List<object>()
                : SqlData.Query("SELECT u.Id AS id, u.Nombre AS nombre, u.Correo AS correo FROM dbo.Inscripcion i JOIN dbo.Usuario u ON u.Id = i.EstudianteId WHERE i.CursoId = @Id ORDER BY u.Nombre",
                    SqlData.Param("@Id", SqlDbType.Int, id));
            return Ok(curso);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear(CursoRequest curso)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "cursos.crear")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para crear cursos.");
            var error = ValidarCurso(curso);
            if (error != null) return Error(HttpStatusCode.BadRequest, error);
            try
            {
                var id = Convert.ToInt32(SqlData.Scalar(
                    "INSERT dbo.Curso (Codigo, Nombre, Periodo, Descripcion, DocenteId) OUTPUT INSERTED.Id VALUES (@Codigo, @Nombre, @Periodo, @Descripcion, @DocenteId)",
                    ParamCurso(curso)));
                SqlData.RegistrarAuditoria(actual.Id, "Creó", "Curso", id, curso.Codigo.Trim());
                return Ok(ObtenerCursoResumen(id));
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe un curso con ese código.");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Actualizar(int id, CursoRequest curso)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "cursos.editar")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para editar cursos.");
            var error = ValidarCurso(curso);
            if (error != null) return Error(HttpStatusCode.BadRequest, error);
            try
            {
                var parametros = ParamCurso(curso).Concat(new[] { SqlData.Param("@Id", SqlDbType.Int, id) }).ToArray();
                var count = SqlData.Execute("UPDATE dbo.Curso SET Codigo=@Codigo, Nombre=@Nombre, Periodo=@Periodo, Descripcion=@Descripcion, DocenteId=@DocenteId WHERE Id=@Id", parametros);
                if (count == 0) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
                SqlData.RegistrarAuditoria(actual.Id, "Actualizó", "Curso", id, curso.Codigo.Trim());
                return Ok(ObtenerCursoResumen(id));
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe un curso con ese código.");
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Eliminar(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "cursos.eliminar")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para eliminar cursos.");
            var archivos = SqlData.Query("SELECT RutaInterna AS ruta FROM dbo.Material WHERE CursoId=@Id UNION ALL SELECT en.RutaInterna AS ruta FROM dbo.Entrega en JOIN dbo.Evaluacion e ON e.Id=en.EvaluacionId WHERE e.CursoId=@Id",
                SqlData.Param("@Id", SqlDbType.Int, id));
            var count = SqlData.Execute("DELETE FROM dbo.Curso WHERE Id=@Id", SqlData.Param("@Id", SqlDbType.Int, id));
            if (count == 0) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            SqlData.RegistrarAuditoria(actual.Id, "Eliminó", "Curso", id, id.ToString());
            foreach (var archivo in archivos) AcademicoController.EliminarArchivoPrivado(Convert.ToString(archivo["ruta"]));
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{id:int}/estudiantes")]
        public IHttpActionResult Matricular(int id, MatriculaRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "cursos.matricular")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para matricular estudiantes.");
            if (request == null || !ExisteCurso(id)) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            var estudiante = AuthService.ObtenerPorId(request.EstudianteId);
            if (estudiante == null || estudiante.Rol != "estudiante" || !estudiante.Activo)
                return Error(HttpStatusCode.BadRequest, "Selecciona un estudiante activo.");
            try
            {
                SqlData.Execute("INSERT dbo.Inscripcion (CursoId, EstudianteId) VALUES (@CursoId, @EstudianteId)",
                    SqlData.Param("@CursoId", SqlDbType.Int, id), SqlData.Param("@EstudianteId", SqlDbType.Int, request.EstudianteId));
                SqlData.RegistrarAuditoria(actual.Id, "Matriculó", "Curso", id, "EstudianteId=" + request.EstudianteId);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "El estudiante ya está matriculado en este curso.");
            }
        }

        [HttpDelete]
        [Route("{id:int}/estudiantes/{estudianteId:int}")]
        public IHttpActionResult Desmatricular(int id, int estudianteId)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "cursos.matricular")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para gestionar matrículas.");
            var existe = Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM dbo.Inscripcion WHERE CursoId=@CursoId AND EstudianteId=@EstudianteId",
                SqlData.Param("@CursoId", SqlDbType.Int, id), SqlData.Param("@EstudianteId", SqlDbType.Int, estudianteId))) > 0;
            if (!existe) return Error(HttpStatusCode.NotFound, "La matrícula no existe.");
            var archivos = SqlData.Query("SELECT en.RutaInterna AS ruta FROM dbo.Entrega en JOIN dbo.Evaluacion e ON e.Id=en.EvaluacionId WHERE e.CursoId=@CursoId AND en.EstudianteId=@EstudianteId",
                SqlData.Param("@CursoId", SqlDbType.Int, id), SqlData.Param("@EstudianteId", SqlDbType.Int, estudianteId));
            SqlData.Execute("DELETE g FROM dbo.Calificacion g JOIN dbo.Evaluacion e ON e.Id=g.EvaluacionId WHERE e.CursoId=@CursoId AND g.EstudianteId=@EstudianteId; DELETE FROM dbo.Asistencia WHERE CursoId=@CursoId AND EstudianteId=@EstudianteId; DELETE en FROM dbo.Entrega en JOIN dbo.Evaluacion e ON e.Id=en.EvaluacionId WHERE e.CursoId=@CursoId AND en.EstudianteId=@EstudianteId",
                SqlData.Param("@CursoId", SqlDbType.Int, id), SqlData.Param("@EstudianteId", SqlDbType.Int, estudianteId));
            var count = SqlData.Execute("DELETE FROM dbo.Inscripcion WHERE CursoId=@CursoId AND EstudianteId=@EstudianteId",
                SqlData.Param("@CursoId", SqlDbType.Int, id), SqlData.Param("@EstudianteId", SqlDbType.Int, estudianteId));
            if (count == 0) return Error(HttpStatusCode.NotFound, "La matrícula no existe.");
            foreach (var archivo in archivos) AcademicoController.EliminarArchivoPrivado(Convert.ToString(archivo["ruta"]));
            SqlData.RegistrarAuditoria(actual.Id, "Retiró matrícula", "Curso", id, "EstudianteId=" + estudianteId);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("{id:int}/mi-curso")]
        public IHttpActionResult MiCurso(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (actual.Rol != "estudiante") return Error(HttpStatusCode.Forbidden, "Esta vista es solo para estudiantes.");
            if (!TieneAcceso(actual.Rol, actual.Id, id)) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");

            var curso = SqlData.Query("SELECT c.Id AS id, c.Codigo AS codigo, c.Nombre AS nombre, c.Descripcion AS descripcion, c.Periodo AS periodo, COALESCE(u.Nombre, N'Sin docente') AS docente FROM dbo.Curso c LEFT JOIN dbo.Usuario u ON u.Id=c.DocenteId WHERE c.Id=@Id",
                SqlData.Param("@Id", SqlDbType.Int, id)).FirstOrDefault();
            var evaluaciones = SqlData.Query(
                "SELECT e.Id AS id, e.CursoId AS cursoId, e.Nombre AS nombre, e.Tipo AS tipo, e.Peso AS peso, CONVERT(VARCHAR(10),e.Fecha,23) AS fecha, e.Instrucciones AS instrucciones, e.RequiereEntrega AS requiereEntrega, e.Extensiones AS extensiones, e.MaxMb AS maxMb, e.PermiteTardia AS permiteTardia, g.Nota AS nota, " +
                "CASE WHEN g.Nota IS NOT NULL THEN 'calificada' WHEN en.Id IS NOT NULL THEN 'entregada' WHEN e.Fecha < CAST(GETDATE() AS DATE) THEN 'vencida' ELSE 'pendiente' END AS estado, " +
                "en.Id AS entregaId, en.NombreOriginal AS entregaNombreOriginal, en.Tamano AS entregaTamano, en.Fecha AS entregaFecha, en.Tardia AS entregaTardia, en.Comentario AS entregaComentario " +
                "FROM dbo.Evaluacion e LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=@EstudianteId LEFT JOIN dbo.Entrega en ON en.EvaluacionId=e.Id AND en.EstudianteId=@EstudianteId WHERE e.CursoId=@CursoId ORDER BY e.Fecha",
                SqlData.Param("@EstudianteId", SqlDbType.Int, actual.Id), SqlData.Param("@CursoId", SqlDbType.Int, id));
            foreach (var evaluacion in evaluaciones)
            {
                if (evaluacion["entregaId"] == null) evaluacion["entrega"] = null;
                else
                {
                    evaluacion["entrega"] = new Dictionary<string, object>
                    {
                        ["id"] = evaluacion["entregaId"], ["evaluacionId"] = evaluacion["id"], ["estudianteId"] = actual.Id,
                        ["nombreOriginal"] = evaluacion["entregaNombreOriginal"], ["tamano"] = evaluacion["entregaTamano"],
                        ["fecha"] = evaluacion["entregaFecha"], ["tardia"] = evaluacion["entregaTardia"], ["comentario"] = evaluacion["entregaComentario"]
                    };
                }
                evaluacion.Remove("entregaId"); evaluacion.Remove("entregaNombreOriginal"); evaluacion.Remove("entregaTamano");
                evaluacion.Remove("entregaFecha"); evaluacion.Remove("entregaTardia"); evaluacion.Remove("entregaComentario");
            }
            var promedio = SqlData.Scalar("SELECT SUM(g.Nota * e.Peso) / NULLIF(SUM(CASE WHEN g.Nota IS NOT NULL THEN e.Peso END),0) FROM dbo.Evaluacion e LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=@Id WHERE e.CursoId=@CursoId",
                SqlData.Param("@Id", SqlDbType.Int, actual.Id), SqlData.Param("@CursoId", SqlDbType.Int, id));
            var asistencia = SqlData.Scalar("SELECT 100.0 * SUM(CASE WHEN Presente=1 THEN 1 ELSE 0 END) / NULLIF(COUNT(*),0) FROM dbo.Asistencia WHERE CursoId=@CursoId AND EstudianteId=@Id",
                SqlData.Param("@CursoId", SqlDbType.Int, id), SqlData.Param("@Id", SqlDbType.Int, actual.Id));
            var promedioDecimal = DecimalNullable(promedio);
            var asistenciaDecimal = DecimalNullable(asistencia);
            return Ok(new { curso, promedio = promedioDecimal, promedioGrupo = MediaGrupo(id),
                asistencia = asistenciaDecimal, riesgo = NivelRiesgo(promedioDecimal, asistenciaDecimal), evaluaciones });
        }

        private static bool TieneAcceso(string rol, int usuarioId, int cursoId)
        {
            return AuthService.ObtenerCursos(rol, usuarioId).Cast<Dictionary<string, object>>().Any(c => Convert.ToInt32(c["id"]) == cursoId);
        }

        private static bool ExisteCurso(int id) => Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM dbo.Curso WHERE Id=@Id", SqlData.Param("@Id", SqlDbType.Int, id))) > 0;

        private static object ObtenerCursoResumen(int id) => AuthService.ObtenerCursos("admin", 0).Cast<Dictionary<string, object>>().FirstOrDefault(c => Convert.ToInt32(c["id"]) == id);

        private static string ValidarCurso(CursoRequest curso)
        {
            if (curso == null || string.IsNullOrWhiteSpace(curso.Codigo) || string.IsNullOrWhiteSpace(curso.Nombre) || string.IsNullOrWhiteSpace(curso.Periodo))
                return "Código, nombre y periodo son obligatorios.";
            if (curso.DocenteId.HasValue)
            {
                var docente = AuthService.ObtenerPorId(curso.DocenteId.Value);
                if (docente == null || docente.Rol != "docente" || !docente.Activo) return "Selecciona un docente activo.";
            }
            return null;
        }

        private static decimal? MediaGrupo(int cursoId)
        {
            var valor = SqlData.Scalar("SELECT AVG(CAST(p.Promedio AS FLOAT)) FROM dbo.Inscripcion i OUTER APPLY (SELECT SUM(g.Nota*e.Peso)/NULLIF(SUM(CASE WHEN g.Nota IS NOT NULL THEN e.Peso END),0) AS Promedio FROM dbo.Evaluacion e LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=i.EstudianteId WHERE e.CursoId=@CursoId) p WHERE i.CursoId=@CursoId",
                SqlData.Param("@CursoId", SqlDbType.Int, cursoId));
            return DecimalNullable(valor);
        }

        private static decimal? DecimalNullable(object value) => value == null || value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(value);

        private static string NivelRiesgo(decimal? promedio, decimal? asistencia)
        {
            if (!promedio.HasValue && !asistencia.HasValue) return "sin-datos";
            if ((promedio.HasValue && promedio.Value < 70) || (asistencia.HasValue && asistencia.Value < 75)) return "alto";
            if ((promedio.HasValue && promedio.Value < 80) || (asistencia.HasValue && asistencia.Value < 85)) return "moderado";
            return "bajo";
        }

        private static SqlParameter[] ParamCurso(CursoRequest curso)
        {
            return new[]
            {
                SqlData.Param("@Codigo", SqlDbType.NVarChar, curso.Codigo.Trim(), 20),
                SqlData.Param("@Nombre", SqlDbType.NVarChar, curso.Nombre.Trim(), 150),
                SqlData.Param("@Periodo", SqlDbType.NVarChar, curso.Periodo.Trim(), 20),
                SqlData.Param("@Descripcion", SqlDbType.NVarChar, curso.Descripcion, 500),
                SqlData.Param("@DocenteId", SqlDbType.Int, curso.DocenteId)
            };
        }
    }
}
