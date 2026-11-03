using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HomeClassApi.Models;
using HomeClassApi.Services;

namespace HomeClassApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private IHttpActionResult Error(HttpStatusCode status, string mensaje)
        {
            return ResponseMessage(Request.CreateResponse(status, new RespuestaError { Mensaje = mensaje }));
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Clave))
                return Error(HttpStatusCode.BadRequest, "Correo y contraseña son obligatorios.");

            var usuario = BuscarUsuario(
                "WHERE u.correo = @Correo",
                SqlData.Param("@Correo", SqlDbType.NVarChar, request.Correo.Trim(), 150));
            if (usuario == null || !usuario.Activo || !AuthService.VerificarClave(request.Clave, usuario.ClaveHash))
                return Error(HttpStatusCode.Unauthorized, "Correo o contraseña incorrectos.");

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            SqlData.Execute(
                "INSERT Usuarios.sesiones (usuario_id, token, expira_en) VALUES (@UsuarioId, @Token, @Expira)",
                SqlData.Param("@UsuarioId", SqlDbType.Int, usuario.Id),
                SqlData.Param("@Token", SqlDbType.NVarChar, token, 255),
                SqlData.Param("@Expira", SqlDbType.DateTime2, DateTime.UtcNow.AddHours(12)));
            SqlData.Execute("UPDATE Usuarios.usuarios SET ultimo_acceso = SYSUTCDATETIME() WHERE id = @Id",
                SqlData.Param("@Id", SqlDbType.Int, usuario.Id));

            return Ok(new { token, usuario = Respuesta(usuario) });
        }

        [HttpGet]
        [Route("sesion")]
        public IHttpActionResult Sesion()
        {
            var usuario = ObtenerUsuarioActual();
            if (usuario == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            return Ok(usuario);
        }

        [HttpPost]
        [Route("salir")]
        public IHttpActionResult Salir()
        {
            var token = ObtenerTokenActual();
            if (token != null)
                SqlData.Execute("DELETE FROM Usuarios.sesiones WHERE token = @Token",
                    SqlData.Param("@Token", SqlDbType.NVarChar, token, 255));
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("solicitar-restablecimiento")]
        public IHttpActionResult SolicitarRestablecimiento(LoginRequest request)
        {
            return Error(HttpStatusCode.NotImplemented,
                "La base de datos actual no incluye un procedimiento para solicitar el restablecimiento de contraseña.");
        }

        internal static UsuarioRespuesta ObtenerUsuarioActual()
        {
            var token = ObtenerTokenActual();
            if (token == null) return null;

            var usuario = BuscarUsuario(
                "JOIN Usuarios.sesiones s ON s.usuario_id = u.id WHERE s.token = @Token AND s.expira_en > SYSUTCDATETIME() AND u.activo = 1",
                SqlData.Param("@Token", SqlDbType.NVarChar, token, 255));
            return usuario == null ? null : Respuesta(usuario);
        }

        private static string ObtenerTokenActual()
        {
            var authorization = HttpContext.Current?.Request?.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authorization) ||
                !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;
            var token = authorization.Substring("Bearer ".Length).Trim();
            return token.Length == 0 ? null : token;
        }

        private static UsuarioSeed BuscarUsuario(string filtro, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            var sql = "SELECT TOP (1) u.id AS Id, u.correo AS Correo, u.password_hash AS ClaveHash, CAST(COALESCE(u.activo, 1) AS BIT) AS Activo, " +
                "CASE LOWER(r.nombre) WHEN N'administrador' THEN N'admin' WHEN N'docente' THEN N'docente' " +
                "WHEN N'estudiante' THEN N'estudiante' ELSE N'' END AS Rol, " +
                "COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(e.nombre, N' ', e.apellido))), N''), " +
                "NULLIF(LTRIM(RTRIM(CONCAT(d.nombre, N' ', d.apellido))), N''), u.correo) AS Nombre " +
                "FROM Usuarios.usuarios u JOIN Usuarios.roles r ON r.id = u.rol_id " +
                "LEFT JOIN Usuarios.estudiantes e ON e.usuario_id = u.id " +
                "LEFT JOIN Usuarios.docentes d ON d.usuario_id = u.id " + filtro;
            var row = SqlData.Query(sql, parameters).FirstOrDefault();
            if (row == null || string.IsNullOrEmpty(Convert.ToString(row["Rol"]))) return null;
            return new UsuarioSeed
            {
                Id = Convert.ToInt32(row["Id"]),
                Nombre = Convert.ToString(row["Nombre"]),
                Correo = Convert.ToString(row["Correo"]),
                ClaveHash = Convert.ToString(row["ClaveHash"]),
                Rol = Convert.ToString(row["Rol"]),
                Activo = Convert.ToBoolean(row["Activo"]),
                TipoAdmin = Convert.ToString(row["Rol"]) == "admin" ? "general" : null,
                PermisosAdminCsv = Convert.ToString(row["Rol"]) == "admin" ? "general" : null
            };
        }

        private static UsuarioRespuesta Respuesta(UsuarioSeed usuario)
        {
            return new UsuarioRespuesta
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Activo = usuario.Activo,
                Bloqueado = false,
                SolicitudRestablecimiento = false,
                TipoAdmin = usuario.TipoAdmin,
                PermisosAdmin = usuario.Rol == "admin" ? new List<string> { "general" } : new List<string>()
            };
        }
    }
}
