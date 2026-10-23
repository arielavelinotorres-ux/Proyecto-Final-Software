using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeClassApi.Controllers;
using HomeClassApi.Models;
using HomeClassApi.Services;

namespace HomeClassApi.Controllers
{
    [RoutePrefix("api/admin")]
    public class AdminController : ApiController
    {
        [HttpGet]
        [Route("auditoria")]
        public IHttpActionResult Auditoria(int limite = 100)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            var permisos = actual.PermisosAdmin ?? new System.Collections.Generic.List<string>();
            if (!AuthService.TienePermisoAdmin(actual, "auditoria.consultar") && !permisos.Contains("reportes")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para consultar auditoría.");
            limite = limite < 1 ? 1 : limite > 300 ? 300 : limite;
            return Ok(SqlData.Query("SELECT TOP (@Limite) a.Id AS id,a.UsuarioId AS usuarioId,COALESCE(u.Nombre,N'Cuenta eliminada') AS usuario,a.Accion AS accion,a.Entidad AS entidad,a.EntidadId AS entidadId,a.Detalle AS detalle,a.Fecha AS fecha FROM dbo.Auditoria a LEFT JOIN dbo.Usuario u ON u.Id=a.UsuarioId ORDER BY a.Fecha DESC,a.Id DESC",
                SqlData.Param("@Limite", SqlDbType.Int, limite)));
        }

        private IHttpActionResult Error(HttpStatusCode status, string mensaje)
        {
            return ResponseMessage(Request.CreateResponse(status, new RespuestaError { Mensaje = mensaje }));
        }
    }
}