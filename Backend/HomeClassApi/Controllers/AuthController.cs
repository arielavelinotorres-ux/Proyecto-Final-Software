using System;
using System.Collections.Generic;
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
        private static readonly Dictionary<string, UsuarioRespuesta> Tokens = new Dictionary<string, UsuarioRespuesta>(StringComparer.OrdinalIgnoreCase);

        private IHttpActionResult Error(HttpStatusCode status, string mensaje)
        {
            return ResponseMessage(Request.CreateResponse(status, new RespuestaError { Mensaje = mensaje }));
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Clave))
            {
                return Error(HttpStatusCode.BadRequest, "Correo y contraseña son obligatorios.");
            }

            var cuenta = AuthService.BuscarPorCorreo(request.Correo);
            if (cuenta != null && !cuenta.Activo)
            {
                return Error(HttpStatusCode.Forbidden, "Tu cuenta está desactivada. Contacta al administrador.");
            }

            var usuario = AuthService.ValidarLogin(request.Correo, request.Clave, out var bloqueado);
            if (usuario == null)
            {
                if (bloqueado)
                {
                    return Error(HttpStatusCode.Forbidden, "Tu cuenta se bloqueó después de cinco intentos fallidos. Debes restablecer tu contraseña con el administrador.");
                }
                return Error(HttpStatusCode.Unauthorized, "Correo o contraseña incorrectos.");
            }

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            lock (Tokens)
            {
                Tokens[token] = usuario;
            }

            return Ok(new
            {
                token,
                usuario = usuario
            });
        }

        [HttpPost]
        [Route("solicitar-restablecimiento")]
        public IHttpActionResult SolicitarRestablecimiento(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo))
            {
                return Error(HttpStatusCode.BadRequest, "Ingresa el correo de tu cuenta.");
            }

            AuthService.SolicitarRestablecimiento(request.Correo);
            return Ok(new { mensaje = "Si la cuenta corresponde a un estudiante o docente, la solicitud fue enviada al administrador." });
        }

        [HttpGet]
        [Route("sesion")]
        public IHttpActionResult Sesion()
        {
            var usuario = ObtenerUsuarioActual();
            if (usuario == null)
            {
                return Error(HttpStatusCode.Unauthorized, "Tu sesión expiró. Ingresa de nuevo.");
            }

            return Ok(usuario);
        }

        internal static UsuarioRespuesta ObtenerUsuarioActual()
        {
            var request = HttpContext.Current?.Request;
            if (request == null) return null;

            var header = request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(header)) return null;

            if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;

            var token = header.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrWhiteSpace(token)) return null;

            lock (Tokens)
            {
                if (Tokens.TryGetValue(token, out var usuario))
                {
                    var vigente = AuthService.ObtenerPorId(usuario.Id);
                    if (vigente == null || !vigente.Activo || vigente.Bloqueado)
                    {
                        Tokens.Remove(token);
                        return null;
                    }
                    Tokens[token] = vigente;
                    return vigente;
                }
            }

            return null;
        }

        internal static void ActualizarSesion(UsuarioRespuesta usuario)
        {
            if (usuario == null) return;
            lock (Tokens)
            {
                foreach (var token in new List<string>(Tokens.Keys))
                {
                    if (Tokens[token].Id == usuario.Id) Tokens[token] = usuario;
                }
            }
        }
    }
}
