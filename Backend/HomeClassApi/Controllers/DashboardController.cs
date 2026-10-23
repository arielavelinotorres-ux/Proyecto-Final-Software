using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeClassApi.Controllers;
using HomeClassApi.Models;
using HomeClassApi.Services;

namespace HomeClassApi.Controllers
{
    [RoutePrefix("api")]
    public class DashboardController : ApiController
    {
        private IHttpActionResult Error(HttpStatusCode status, string mensaje)
        {
            return ResponseMessage(Request.CreateResponse(status, new RespuestaError { Mensaje = mensaje }));
        }

        [HttpGet]
        [Route("dashboard")]
        public IHttpActionResult GetDashboard()
        {
            var usuario = AuthController.ObtenerUsuarioActual();
            if (usuario == null)
            {
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            }

            if (usuario.Rol == "admin" && !AuthService.TienePermisoAdmin(usuario, "general"))
                return Error(HttpStatusCode.Forbidden, "El resumen institucional está reservado para administración general.");

            var usuarios = AuthService.ObtenerUsuarios();
            var cursos = AuthService.ObtenerCursos(usuario.Rol, usuario.Id).Cast<Dictionary<string, object>>().ToList();
            foreach (var curso in cursos)
            {
                var metricas = MetricasCurso(Convert.ToInt32(curso["id"]));
                curso["promedio"] = metricas.promedio;
                curso["asistencia"] = metricas.asistencia;
                curso["enRiesgo"] = metricas.alto;
                if (usuario.Rol == "estudiante")
                {
                    var propias = MetricasEstudiante(Convert.ToInt32(curso["id"]), usuario.Id);
                    var grupo = PromedioGrupo(Convert.ToInt32(curso["id"]));
                    curso["promedio"] = propias.promedio;
                    curso["asistencia"] = propias.asistencia;
                    curso["promedioGrupo"] = grupo;
                    curso["riesgo"] = Riesgo(propias.promedio, propias.asistencia);
                }
            }

            if (usuario.Rol == "admin")
            {
                return Ok(new
                {
                    rol = usuario.Rol,
                    usuarios = new
                    {
                        total = usuarios.Count,
                        admins = usuarios.Count(u => u.Rol == "admin"),
                        docentes = usuarios.Count(u => u.Rol == "docente"),
                        estudiantes = usuarios.Count(u => u.Rol == "estudiante"),
                        inactivos = usuarios.Count(u => !u.Activo)
                    },
                    cursos,
                    promedioGeneral = Media(cursos.Select(c => c["promedio"])),
                    asistenciaGeneral = Media(cursos.Select(c => c["asistencia"])),
                    riesgo = RiesgosInstitucionales()
                });
            }

            if (usuario.Rol == "docente")
            {
                var estudiantes = new List<Dictionary<string, object>>();
                foreach (var curso in cursos)
                {
                    var cursoId = Convert.ToInt32(curso["id"]);
                    var filas = SqlData.Query("SELECT u.Id AS estudianteId,u.Nombre AS nombre FROM dbo.Inscripcion i JOIN dbo.Usuario u ON u.Id=i.EstudianteId WHERE i.CursoId=@CursoId",
                        SqlData.Param("@CursoId", SqlDbType.Int, cursoId));
                    foreach (var fila in filas)
                    {
                        var metrica = MetricasEstudiante(cursoId, Convert.ToInt32(fila["estudianteId"]));
                        var riesgo = Riesgo(metrica.promedio, metrica.asistencia);
                        if (riesgo == "alto" || riesgo == "moderado")
                            estudiantes.Add(new Dictionary<string, object>
                            {
                                ["estudianteId"] = fila["estudianteId"], ["nombre"] = fila["nombre"], ["cursoId"] = cursoId,
                                ["curso"] = curso["nombre"], ["riesgo"] = riesgo, ["promedio"] = metrica.promedio, ["asistencia"] = metrica.asistencia
                            });
                    }
                }
                var porRevisar = Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM dbo.Entrega e JOIN dbo.Evaluacion v ON v.Id=e.EvaluacionId JOIN dbo.Curso c ON c.Id=v.CursoId LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.EvaluacionId AND g.EstudianteId=e.EstudianteId WHERE c.DocenteId=@Id AND g.Nota IS NULL",
                    SqlData.Param("@Id", SqlDbType.Int, usuario.Id)));
                var proximas = SqlData.Query("SELECT TOP 6 e.Id AS evaluacionId,c.Id AS cursoId,c.Nombre AS curso,e.Nombre AS nombre,e.Tipo AS tipo,CONVERT(VARCHAR(10),e.Fecha,23) AS fecha FROM dbo.Evaluacion e JOIN dbo.Curso c ON c.Id=e.CursoId WHERE c.DocenteId=@Id AND e.Fecha>=CAST(GETDATE() AS DATE) ORDER BY e.Fecha",
                    SqlData.Param("@Id", SqlDbType.Int, usuario.Id));
                return Ok(new
                {
                    rol = usuario.Rol,
                    cursos,
                    porRevisar,
                    enRiesgoAlto = estudiantes.Count(e => Equals(e["riesgo"], "alto")),
                    promedioGeneral = Media(cursos.Select(c => c["promedio"])),
                    alertas = estudiantes.Take(8),
                    proximas
                });
            }

            var promedioEstudiante = Media(cursos.Select(c => c["promedio"]));
            var asistenciaEstudiante = Media(cursos.Select(c => c["asistencia"]));
            var pendientes = SqlData.Query("SELECT e.Id AS evaluacionId,c.Id AS cursoId,c.Nombre AS curso,e.Nombre AS nombre,e.Tipo AS tipo,CONVERT(VARCHAR(10),e.Fecha,23) AS fecha,CASE WHEN e.Fecha<CAST(GETDATE() AS DATE) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS vencida FROM dbo.Inscripcion i JOIN dbo.Curso c ON c.Id=i.CursoId JOIN dbo.Evaluacion e ON e.CursoId=c.Id LEFT JOIN dbo.Entrega en ON en.EvaluacionId=e.Id AND en.EstudianteId=i.EstudianteId LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=i.EstudianteId WHERE i.EstudianteId=@Id AND e.RequiereEntrega=1 AND en.Id IS NULL AND g.Nota IS NULL ORDER BY e.Fecha",
                SqlData.Param("@Id", SqlDbType.Int, usuario.Id));
            return Ok(new
            {
                rol = usuario.Rol,
                promedio = promedioEstudiante,
                asistencia = asistenciaEstudiante,
                cursos,
                pendientes
            });
        }

        private static (double? promedio, double? asistencia, int alto) MetricasCurso(int cursoId)
        {
            var rows = SqlData.Query("SELECT i.EstudianteId AS id FROM dbo.Inscripcion i WHERE i.CursoId=@CursoId", SqlData.Param("@CursoId", SqlDbType.Int, cursoId));
            var metricas = rows.Select(r => MetricasEstudiante(cursoId, Convert.ToInt32(r["id"]))).ToList();
            return (Media(metricas.Select(m => (object)m.promedio)), Media(metricas.Select(m => (object)m.asistencia)), metricas.Count(m => Riesgo(m.promedio, m.asistencia) == "alto"));
        }

        private static (double? promedio, double? asistencia) MetricasEstudiante(int cursoId, int estudianteId)
        {
            var row = SqlData.Query("SELECT (SELECT SUM(g.Nota*e.Peso)/NULLIF(SUM(CASE WHEN g.Nota IS NOT NULL THEN e.Peso END),0) FROM dbo.Evaluacion e LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=@EstudianteId WHERE e.CursoId=@CursoId) AS promedio,(SELECT 100.0*SUM(CASE WHEN a.Presente=1 THEN 1 ELSE 0 END)/NULLIF(COUNT(*),0) FROM dbo.Asistencia a WHERE a.CursoId=@CursoId AND a.EstudianteId=@EstudianteId) AS asistencia",
                SqlData.Param("@EstudianteId", SqlDbType.Int, estudianteId), SqlData.Param("@CursoId", SqlDbType.Int, cursoId)).FirstOrDefault();
            return (Numero(row["promedio"]), Numero(row["asistencia"]));
        }

        private static double? PromedioGrupo(int cursoId) => Media(SqlData.Query("SELECT i.EstudianteId AS id FROM dbo.Inscripcion i WHERE i.CursoId=@CursoId", SqlData.Param("@CursoId", SqlDbType.Int, cursoId))
            .Select(r => (object)MetricasEstudiante(cursoId, Convert.ToInt32(r["id"])).promedio));

        private static object RiesgosInstitucionales()
        {
            var filas = SqlData.Query("SELECT c.Id AS cursoId,i.EstudianteId AS estudianteId FROM dbo.Inscripcion i JOIN dbo.Curso c ON c.Id=i.CursoId");
            var riesgos = filas.Select(r =>
            {
                var metrica = MetricasEstudiante(Convert.ToInt32(r["cursoId"]), Convert.ToInt32(r["estudianteId"]));
                return Riesgo(metrica.promedio, metrica.asistencia);
            }).ToList();
            return new { alto = riesgos.Count(x => x == "alto"), moderado = riesgos.Count(x => x == "moderado"), bajo = riesgos.Count(x => x == "bajo") };
        }

        private static string Riesgo(double? promedio, double? asistencia)
        {
            if (!promedio.HasValue && !asistencia.HasValue) return "sin-datos";
            if ((promedio.HasValue && promedio < 70) || (asistencia.HasValue && asistencia < 75)) return "alto";
            if ((promedio.HasValue && promedio < 80) || (asistencia.HasValue && asistencia < 85)) return "moderado";
            return "bajo";
        }

        private static double? Numero(object value) => value == null || value == DBNull.Value ? (double?)null : Convert.ToDouble(value);

        private static double? Media(IEnumerable<object> values)
        {
            var numericos = values.Select(Numero).Where(v => v.HasValue).Select(v => v.Value).ToList();
            return numericos.Count == 0 ? (double?)null : Math.Round(numericos.Average(), 1);
        }
    }
}
