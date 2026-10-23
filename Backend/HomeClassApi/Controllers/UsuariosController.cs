using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using HomeClassApi.Controllers;
using HomeClassApi.Models;
using HomeClassApi.Services;

namespace HomeClassApi.Controllers
{
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
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

            if (!AuthService.TienePermisoAdmin(usuarioActual, "usuarios") && !AuthService.TienePermisoAdmin(usuarioActual, "reportes") && !AuthService.TienePermisoAdmin(usuarioActual, "cursos"))
            {
                return Error(HttpStatusCode.Forbidden, "No tienes permisos para consultar usuarios.");
            }

            var usuarios = AuthService.ObtenerUsuarios();
            if (!AuthService.TienePermisoAdmin(usuarioActual, "general") && !AuthService.TienePermisoAdmin(usuarioActual, "reportes"))
                return Ok(usuarios.Where(u => u.Activo && u.Rol != "admin").Select(u => new { u.Id, u.Nombre, u.Correo, u.Rol, u.Activo }));
            return Ok(usuarios);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear(UsuarioRequest usuario)
        {
            var usuarioActual = AuthController.ObtenerUsuarioActual();
            if (usuarioActual == null)
            {
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            }

            if (!AuthService.TienePermisoAdmin(usuarioActual, "usuarios.crear"))
                return Error(HttpStatusCode.Forbidden, "No tienes permiso para crear usuarios.");

            var permisosAdmin = usuario?.Rol == "admin" ? AuthService.NormalizarPermisosAdmin(usuario.PermisosAdmin, usuario.TipoAdmin) : new List<string>();
            var tipoAdmin = usuario?.Rol == "admin" ? (permisosAdmin.Contains("general") ? "general" : "personalizado") : "general";
            if (usuario?.Rol == "admin" && !AuthService.TienePermisoAdmin(usuarioActual, "general"))
                return Error(HttpStatusCode.Forbidden, "Solo un administrador general puede crear cuentas administrativas.");

            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Nombre) || string.IsNullOrWhiteSpace(usuario.Correo) ||
                string.IsNullOrWhiteSpace(usuario.Clave) || usuario.Clave.Length < 8 ||
                !new[] { "admin", "docente", "estudiante" }.Contains(usuario.Rol) ||
                (usuario.Rol == "admin" && !AuthService.PermisosAdminValidos(permisosAdmin)))
            {
                return Error(HttpStatusCode.BadRequest, "Revisa los datos del usuario; la contraseña debe tener al menos 8 caracteres.");
            }

            try
            {
                var id = Convert.ToInt32(SqlData.Scalar(
                    "INSERT dbo.Usuario (Nombre, Correo, ClaveHash, Rol, Activo, TipoAdmin, PermisosAdmin) OUTPUT INSERTED.Id VALUES (@Nombre, @Correo, @ClaveHash, @Rol, 1, @TipoAdmin, @PermisosAdmin)",
                    SqlData.Param("@Nombre", SqlDbType.NVarChar, usuario.Nombre.Trim(), 100),
                    SqlData.Param("@Correo", SqlDbType.NVarChar, usuario.Correo.Trim(), 254),
                    SqlData.Param("@ClaveHash", SqlDbType.VarChar, AuthService.Hash(usuario.Clave), 120),
                    SqlData.Param("@Rol", SqlDbType.VarChar, usuario.Rol, 20),
                    SqlData.Param("@TipoAdmin", SqlDbType.VarChar, tipoAdmin, 20),
                    SqlData.Param("@PermisosAdmin", SqlDbType.VarChar, usuario.Rol == "admin" ? string.Join(",", permisosAdmin) : null, 500)));
                SqlData.RegistrarAuditoria(usuarioActual.Id, "Creó", "Usuario", id, usuario.Correo.Trim());
                return Ok(AuthService.ObtenerPorId(id));
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe un usuario con ese correo.");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Actualizar(int id, UsuarioRequest usuario)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "usuarios.editar")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para editar usuarios.");
            var existente = AuthService.ObtenerPorId(id);
            if (existente == null) return Error(HttpStatusCode.NotFound, "Usuario no encontrado.");
            if ((existente.Rol == "admin" || usuario?.Rol == "admin") && !AuthService.TienePermisoAdmin(actual, "general"))
                return Error(HttpStatusCode.Forbidden, "Solo un administrador general puede modificar perfiles administrativos.");
            var permisosAdmin = usuario?.Rol == "admin" ? AuthService.NormalizarPermisosAdmin(usuario.PermisosAdmin, usuario.TipoAdmin) : new List<string>();
            var tipoAdmin = usuario?.Rol == "admin" ? (permisosAdmin.Contains("general") ? "general" : "personalizado") : "general";
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Nombre) || string.IsNullOrWhiteSpace(usuario.Correo) ||
                !new[] { "admin", "docente", "estudiante" }.Contains(usuario.Rol) ||
                (usuario.Rol == "admin" && !AuthService.PermisosAdminValidos(permisosAdmin)))
                return Error(HttpStatusCode.BadRequest, "Revisa los datos del usuario.");
            if (id == actual.Id && (!usuario.Activo || usuario.Rol != "admin" || !AuthService.PermisosAdminEquivalentes(actual, permisosAdmin)))
                return Error(HttpStatusCode.Conflict, "No puedes desactivar tu cuenta ni reducir tu perfil administrativo.");

            try
            {
                var count = SqlData.Execute(
                    "UPDATE dbo.Usuario SET Nombre = @Nombre, Correo = @Correo, Rol = @Rol, Activo = @Activo, TipoAdmin = @TipoAdmin, PermisosAdmin = @PermisosAdmin WHERE Id = @Id",
                    SqlData.Param("@Nombre", SqlDbType.NVarChar, usuario.Nombre.Trim(), 100),
                    SqlData.Param("@Correo", SqlDbType.NVarChar, usuario.Correo.Trim(), 254),
                    SqlData.Param("@Rol", SqlDbType.VarChar, usuario.Rol, 20),
                    SqlData.Param("@Activo", SqlDbType.Bit, usuario.Activo),
                    SqlData.Param("@TipoAdmin", SqlDbType.VarChar, tipoAdmin, 20),
                    SqlData.Param("@PermisosAdmin", SqlDbType.VarChar, usuario.Rol == "admin" ? string.Join(",", permisosAdmin) : null, 500),
                    SqlData.Param("@Id", SqlDbType.Int, id));
                if (count == 0) return Error(HttpStatusCode.NotFound, "Usuario no encontrado.");
                SqlData.RegistrarAuditoria(actual.Id, "Actualizó", "Usuario", id, usuario.Correo.Trim());
                return Ok(AuthService.ObtenerPorId(id));
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return Error(HttpStatusCode.Conflict, "Ya existe un usuario con ese correo.");
            }
        }

        [HttpPost]
        [Route("{id:int}/restablecer-clave")]
        public IHttpActionResult RestablecerClave(int id, RestablecerClaveRequest request)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "usuarios.restablecer")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para restablecer contraseñas.");
            if (request == null || string.IsNullOrWhiteSpace(request.Nueva) || request.Nueva.Length < 8)
                return Error(HttpStatusCode.BadRequest, "La nueva contraseña debe tener al menos 8 caracteres.");
            if (!AuthService.RestablecerClave(id, request.Nueva))
                return Error(HttpStatusCode.NotFound, "Solo se pueden restablecer cuentas de estudiantes o docentes.");

            SqlData.RegistrarAuditoria(actual.Id, "Restableció clave", "Usuario", id, "Se desbloqueó la cuenta y se restableció la contraseña.");
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Eliminar(int id)
        {
            var actual = AuthController.ObtenerUsuarioActual();
            if (actual == null) return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            if (!AuthService.TienePermisoAdmin(actual, "usuarios.eliminar")) return Error(HttpStatusCode.Forbidden, "No tienes permiso para eliminar usuarios.");
            if (id == actual.Id) return Error(HttpStatusCode.Conflict, "No puedes eliminar tu propia cuenta.");
            var usuario = AuthService.ObtenerPorId(id);
            if (usuario == null) return Error(HttpStatusCode.NotFound, "Usuario no encontrado.");
            if (usuario.Rol == "admin" && !AuthService.TienePermisoAdmin(actual, "general"))
                return Error(HttpStatusCode.Forbidden, "Solo administración general puede eliminar cuentas administrativas.");
            if (usuario.Rol == "docente" && Convert.ToInt32(SqlData.Scalar("SELECT COUNT(*) FROM dbo.Curso WHERE DocenteId = @Id", SqlData.Param("@Id", SqlDbType.Int, id))) > 0)
                return Error(HttpStatusCode.Conflict, "No puedes eliminar un docente con cursos asignados.");
            var archivos = usuario.Rol == "estudiante"
                ? SqlData.Query("SELECT RutaInterna AS ruta FROM dbo.Entrega WHERE EstudianteId=@Id", SqlData.Param("@Id", SqlDbType.Int, id))
                : new List<System.Collections.Generic.Dictionary<string, object>>();
            SqlData.Execute("DELETE FROM dbo.Usuario WHERE Id = @Id", SqlData.Param("@Id", SqlDbType.Int, id));
            SqlData.RegistrarAuditoria(actual.Id, "Eliminó", "Usuario", id, usuario.Correo);
            foreach (var archivo in archivos) AcademicoController.EliminarArchivoPrivado(System.Convert.ToString(archivo["ruta"]));
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
