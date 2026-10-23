using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeClassApi.Models;
using HomeClassApi.Services;

namespace HomeClassApi.Controllers
{
    [RoutePrefix("api")]
    public class ComunicacionController : ApiController
    {
        private IHttpActionResult Error(HttpStatusCode status, string mensaje)
        {
            return ResponseMessage(Request.CreateResponse(status, new RespuestaError { Mensaje = mensaje }));
        }

        [HttpGet]
        [Route("cursos/{cursoId:int}/avisos")]
        public IHttpActionResult ListarAvisos(int cursoId)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!PuedeAcceder(actual, cursoId)) return Error(HttpStatusCode.NotFound, "Curso no encontrado.");
            return Ok(SqlData.Query("SELECT a.Id AS id,a.CursoId AS cursoId,a.AutorId AS autorId,COALESCE(u.Nombre,N'Docente') AS autor,a.Titulo AS titulo,a.Mensaje AS mensaje,a.Fecha AS fecha FROM dbo.Aviso a LEFT JOIN dbo.Usuario u ON u.Id=a.AutorId WHERE a.CursoId=@CursoId ORDER BY a.Fecha DESC",
                SqlData.Param("@CursoId", SqlDbType.Int, cursoId)));
        }

        [HttpPost]
        [Route("cursos/{cursoId:int}/avisos")]
        public IHttpActionResult PublicarAviso(int cursoId, AvisoRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!EsDocenteDelCurso(actual.Id, cursoId)) return Error(HttpStatusCode.Forbidden, "Solo el docente asignado puede publicar avisos.");
            if (request == null || string.IsNullOrWhiteSpace(request.Titulo) || request.Titulo.Trim().Length > 120 ||
                string.IsNullOrWhiteSpace(request.Mensaje) || request.Mensaje.Trim().Length > 2000)
                return Error(HttpStatusCode.BadRequest, "El título y el mensaje son obligatorios (máximo 120 y 2000 caracteres).");
            var id = Convert.ToInt32(SqlData.Scalar("INSERT dbo.Aviso (CursoId,AutorId,Titulo,Mensaje) OUTPUT INSERTED.Id VALUES (@CursoId,@AutorId,@Titulo,@Mensaje)",
                SqlData.Param("@CursoId", SqlDbType.Int, cursoId), SqlData.Param("@AutorId", SqlDbType.Int, actual.Id),
                SqlData.Param("@Titulo", SqlDbType.NVarChar, request.Titulo.Trim(), 120), SqlData.Param("@Mensaje", SqlDbType.NVarChar, request.Mensaje.Trim(), 2000)));
            SqlData.RegistrarAuditoria(actual.Id, "Publicó", "Aviso", id, request.Titulo.Trim());
            return Ok(SqlData.Query("SELECT a.Id AS id,a.CursoId AS cursoId,a.AutorId AS autorId,u.Nombre AS autor,a.Titulo AS titulo,a.Mensaje AS mensaje,a.Fecha AS fecha FROM dbo.Aviso a LEFT JOIN dbo.Usuario u ON u.Id=a.AutorId WHERE a.Id=@Id",
                SqlData.Param("@Id", SqlDbType.Int, id)).FirstOrDefault());
        }

        [HttpGet]
        [Route("agenda")]
        public IHttpActionResult Agenda()
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            var enlace = actual.Rol == "estudiante"
                ? "JOIN dbo.Inscripcion i ON i.CursoId=c.Id AND i.EstudianteId=@UsuarioId "
                : string.Empty;
            var filtro = actual.Rol == "docente" ? "WHERE c.DocenteId=@UsuarioId " : string.Empty;
            var evaluaciones = SqlData.Query("SELECT 'evaluacion' AS categoria,e.Id AS id,c.Id AS cursoId,c.Nombre AS curso,e.Nombre AS titulo,e.Tipo AS tipoEvaluacion,CONVERT(VARCHAR(10),e.Fecha,23) AS fecha,e.Instrucciones AS detalle FROM dbo.Evaluacion e JOIN dbo.Curso c ON c.Id=e.CursoId " + enlace + filtro + "ORDER BY e.Fecha",
                SqlData.Param("@UsuarioId", SqlDbType.Int, actual.Id));
            var avisos = SqlData.Query("SELECT 'aviso' AS categoria,a.Id AS id,c.Id AS cursoId,c.Nombre AS curso,a.Titulo AS titulo,CAST(NULL AS VARCHAR(20)) AS tipoEvaluacion,a.Fecha AS fecha,a.Mensaje AS detalle FROM dbo.Aviso a JOIN dbo.Curso c ON c.Id=a.CursoId " + enlace + filtro + "ORDER BY a.Fecha DESC",
                SqlData.Param("@UsuarioId", SqlDbType.Int, actual.Id));
            return Ok(evaluaciones.Concat(avisos).OrderBy(x => Convert.ToDateTime(x["fecha"])));
        }

        [HttpGet]
        [Route("estudiante/historial")]
        public IHttpActionResult Historial()
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (actual.Rol != "estudiante") return Error(HttpStatusCode.Forbidden, "El historial está disponible para estudiantes.");
            return Ok(SqlData.Query("SELECT c.Id AS cursoId,c.Nombre AS curso,e.Id AS evaluacionId,e.Nombre AS evaluacion,e.Tipo AS tipo,e.Fecha AS fecha,e.Peso AS peso,g.Nota AS nota,CASE WHEN g.Nota IS NOT NULL THEN 'calificada' WHEN en.Id IS NOT NULL THEN 'entregada' WHEN e.Fecha<CAST(GETDATE() AS DATE) THEN 'vencida' ELSE 'pendiente' END AS estado,en.NombreOriginal AS archivo,en.Fecha AS fechaEntrega,en.Tardia AS tardia FROM dbo.Inscripcion i JOIN dbo.Curso c ON c.Id=i.CursoId JOIN dbo.Evaluacion e ON e.CursoId=c.Id LEFT JOIN dbo.Calificacion g ON g.EvaluacionId=e.Id AND g.EstudianteId=i.EstudianteId LEFT JOIN dbo.Entrega en ON en.EvaluacionId=e.Id AND en.EstudianteId=i.EstudianteId WHERE i.EstudianteId=@Id ORDER BY e.Fecha DESC",
                SqlData.Param("@Id", SqlDbType.Int, actual.Id)));
        }

        private static bool PuedeAcceder(UsuarioRespuesta usuario, int cursoId)
        {
            if (usuario.Rol == "admin") return AuthService.TienePermisoAdmin(usuario, "cursos") && Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM dbo.Curso WHERE Id=@CursoId", SqlData.Param("@CursoId", SqlDbType.Int, cursoId))) > 0;
            if (usuario.Rol == "docente") return EsDocenteDelCurso(usuario.Id, cursoId);
            return Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM dbo.Inscripcion WHERE CursoId=@CursoId AND EstudianteId=@UsuarioId", SqlData.Param("@CursoId", SqlDbType.Int, cursoId), SqlData.Param("@UsuarioId", SqlDbType.Int, usuario.Id))) > 0;
        }

        private static bool EsDocenteDelCurso(int docenteId, int cursoId) => Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM dbo.Curso WHERE Id=@CursoId AND DocenteId=@DocenteId",
            SqlData.Param("@CursoId", SqlDbType.Int, cursoId), SqlData.Param("@DocenteId", SqlDbType.Int, docenteId))) > 0;
    }
}