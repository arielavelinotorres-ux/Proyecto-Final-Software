using SistemaEducativoDeRiesgo.Application.DTOs;
using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Web.Api.Endpoints

{
    public static class ComunicacionEndpoints
    {
        public static void MapComunicacionEndpoints(this IEndpointRouteBuilder app)
        {
            // ==================== FOROS ====================
            var foros = app.MapGroup("/api/foros").WithTags("Foros");

            foros.MapPost("/", async (CrearForoRequest request, IForoService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/foros/{id}", new IdResponse(id));
            });

            foros.MapGet("/curso/{cursoId:int}", async (int cursoId, IForoService service) =>
            {
                var lista = await service.ListarPorCursoAsync(cursoId);
                return Results.Ok(lista);
            });

            foros.MapPut("/{id:int}", async (int id, ActualizarForoRequest request, IForoService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Foro actualizado correctamente."));
            });

            foros.MapPatch("/{id:int}/archivar", async (int id, IForoService service) =>
            {
                await service.DesactivarAsync(id);
                return Results.Ok(new MensajeResponse("Foro archivado correctamente."));
            });

            // ==================== PUBLICACIONES DE FORO ====================
            var publicaciones = app.MapGroup("/api/publicaciones-foro").WithTags("PublicacionesForo");

            publicaciones.MapPost("/", async (CrearPublicacionForoRequest request, IPublicacionForoService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/publicaciones-foro/{id}", new IdResponse(id));
            });

            publicaciones.MapGet("/foro/{foroId:int}", async (int foroId, IPublicacionForoService service) =>
            {
                var lista = await service.ListarPorForoAsync(foroId);
                return Results.Ok(lista);
            });

            publicaciones.MapPut("/{id:int}", async (int id, ActualizarPublicacionForoRequest request, IPublicacionForoService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Publicación actualizada correctamente."));
            });

            publicaciones.MapDelete("/{id:int}", async (int id, IPublicacionForoService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Publicación eliminada correctamente."));
            });

            // ==================== MENSAJES PRIVADOS ====================
            var mensajes = app.MapGroup("/api/mensajes-privados").WithTags("MensajesPrivados");

            mensajes.MapPost("/", async (CrearMensajePrivadoRequest request, IMensajePrivadoService service) =>
            {
                var id = await service.EnviarAsync(request);
                return Results.Created($"/api/mensajes-privados/{id}", new IdResponse(id));
            });

            mensajes.MapGet("/recibidos/{receptorId:int}", async (int receptorId, IMensajePrivadoService service) =>
            {
                var lista = await service.ListarRecibidosAsync(receptorId);
                return Results.Ok(lista);
            });

            mensajes.MapGet("/enviados/{emisorId:int}", async (int emisorId, IMensajePrivadoService service) =>
            {
                var lista = await service.ListarEnviadosAsync(emisorId);
                return Results.Ok(lista);
            });

            mensajes.MapPatch("/{id:int}/leer", async (int id, IMensajePrivadoService service) =>
            {
                await service.MarcarComoLeidoAsync(id);
                return Results.Ok(new MensajeResponse("Mensaje marcado como leído."));
            });

            mensajes.MapDelete("/{id:int}", async (int id, IMensajePrivadoService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Mensaje eliminado correctamente."));
            });

            // ==================== NOTIFICACIONES ====================
            var notificaciones = app.MapGroup("/api/notificaciones").WithTags("Notificaciones");

            notificaciones.MapPost("/", async (CrearNotificacionRequest request, INotificacionService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/notificaciones/{id}", new IdResponse(id));
            });

            notificaciones.MapGet("/usuario/{usuarioId:int}", async (int usuarioId, INotificacionService service) =>
            {
                var lista = await service.ListarPorUsuarioAsync(usuarioId);
                return Results.Ok(lista);
            });

            notificaciones.MapPatch("/{id:int}/leer", async (int id, INotificacionService service) =>
            {
                await service.MarcarLeidaAsync(id);
                return Results.Ok(new MensajeResponse("Notificación marcada como leída."));
            });

            notificaciones.MapPatch("/usuario/{usuarioId:int}/leer-todas", async (int usuarioId, INotificacionService service) =>
            {
                await service.MarcarTodasLeidasAsync(usuarioId);
                return Results.Ok(new MensajeResponse("Todas las notificaciones fueron marcadas como leídas."));
            });

            notificaciones.MapDelete("/{id:int}", async (int id, INotificacionService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Notificación eliminada correctamente."));
            });
        }
    }
}
