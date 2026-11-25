using SistemaEducativoDeRiesgo.Application.DTOs;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios;


namespace SistemaEducativoDeRiesgo.Web.Api.Endpoints
{
    public static class UsuariosEndpoints
    {
        public static void MapUsuariosEndpoints(this IEndpointRouteBuilder app)
        {

            // ==================== USUARIOS ====================
            var usuarios = app.MapGroup("/api/usuarios").WithTags("Usuarios");

            usuarios.MapPost("/login", async (LoginRequest request, IUsuarioService service) =>
            {
                var resultado = await service.LoginAsync(request);
                return resultado is not null
                    ? Results.Ok(resultado)
                    : Results.Json(new MensajeResponse("Credenciales inválidas o usuario inactivo."), statusCode: StatusCodes.Status401Unauthorized);
            });

            usuarios.MapPost("/", async (CrearUsuarioRequest request, IUsuarioService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/usuarios/{id}", new IdResponse(id));
            });

            usuarios.MapGet("/", async (IUsuarioService service) =>
            {
                var lista = await service.ListarAsync();
                return Results.Ok(lista);
            });

            usuarios.MapPut("/{id:int}/perfil", async (int id, ActualizarUsuarioPerfilRequest request, IUsuarioService service) =>
            {
                await service.ActualizarPerfilAsync(id, request);
                return Results.Ok(new MensajeResponse("Perfil de usuario actualizado correctamente."));
            });

            usuarios.MapPut("/{id:int}/password", async (int id, ActualizarPasswordRequest request, IUsuarioService service) =>
            {
                await service.ActualizarPasswordAsync(id, request);
                return Results.Ok(new MensajeResponse("Contraseña actualizada correctamente."));
            });

            usuarios.MapPatch("/{id:int}/desactivar", async (int id, IUsuarioService service) =>
            {
                await service.DesactivarAsync(id);
                return Results.Ok(new MensajeResponse("Usuario desactivado correctamente."));
            });

            usuarios.MapPatch("/{id:int}/activar", async (int id, IUsuarioService service) =>
            {
                await service.ActivarAsync(id);
                return Results.Ok(new MensajeResponse("Usuario activado correctamente."));
            });

            usuarios.MapPost("/{id:int}/cerrar-sesiones", async (int id, IUsuarioService service) =>
            {
                await service.CerrarTodasSesionesAsync(id);
                return Results.Ok(new MensajeResponse("Todas las sesiones del usuario fueron cerradas."));
            });



            // ==================== DOCENTES ====================
            var docentes = app.MapGroup("/api/docentes").WithTags("Docentes");

            docentes.MapPost("/", async (CrearDocenteRequest request, IDocenteService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/docentes/{id}", new IdResponse(id));
            });

            docentes.MapGet("/", async (IDocenteService service) =>
            {
                var lista = await service.ListarAsync();
                return Results.Ok(lista);
            });

            docentes.MapPut("/{id:int}", async (int id, ActualizarDocenteRequest request, IDocenteService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Docente actualizado correctamente."));
            });

            docentes.MapDelete("/{id:int}", async (int id, IDocenteService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Docente eliminado correctamente."));
            });



            // ==================== ESTUDIANTES ====================
            var estudiantes = app.MapGroup("/api/estudiantes").WithTags("Estudiantes");

            estudiantes.MapPost("/", async (CrearEstudianteRequest request, IEstudianteService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/estudiantes/{id}", new IdResponse(id));
            });

            estudiantes.MapGet("/", async (IEstudianteService service) =>
            {
                var lista = await service.ListarAsync();
                return Results.Ok(lista);
            });

            estudiantes.MapGet("/usuario/{usuarioId:int}", async (int usuarioId, IEstudianteService service) =>
            {
                var estudiante = await service.ObtenerPorUsuarioIdAsync(usuarioId);
                return estudiante is not null
                    ? Results.Ok(estudiante)
                    : Results.NotFound(new MensajeResponse("No se encontró el perfil de estudiante."));
            });

            estudiantes.MapPut("/{id:int}", async (int id, ActualizarEstudianteRequest request, IEstudianteService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Estudiante actualizado correctamente."));
            });

            estudiantes.MapDelete("/{id:int}", async (int id, IEstudianteService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Estudiante eliminado correctamente."));
            });




            // ==================== RECUPERACIÓN DE CONTRASEÑAS ====================
            var recuperacion = app.MapGroup("/api/recuperacion-password").WithTags("RecuperacionPassword");

            recuperacion.MapPost("/generar", async (GenerarTokenRecuperacionRequest request, IRecuperacionPasswordService service) =>
            {
                var token = await service.GenerarTokenAsync(request);
                return Results.Created($"/api/recuperacion-password/{token.Id}", token);
            });

            recuperacion.MapPost("/validar", async (ValidarTokenRequest request, IRecuperacionPasswordService service) =>
            {
                var resultado = await service.ValidarTokenAsync(request.Token);
                return resultado is not null
                    ? Results.Ok(resultado)
                    : Results.NotFound(new MensajeResponse("El token es inválido o ha expirado."));
            });

            recuperacion.MapPatch("/{id:int}/usar", async (int id, IRecuperacionPasswordService service) =>
            {
                await service.MarcarTokenUsadoAsync(id);
                return Results.Ok(new MensajeResponse("Token marcado como usado."));
            });
        }


    }
}
