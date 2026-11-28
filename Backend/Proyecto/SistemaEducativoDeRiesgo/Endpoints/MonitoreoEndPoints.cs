using SistemaEducativoDeRiesgo.Application.DTOs;
using SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo;
using SistemaEducativoDeRiesgo.Application.Interfaces.Monitoreo;

namespace SistemaEducativoDeRiesgo.Web.Api.Endpoints
{
    public static class MonitoreoEndpoints
    {
        public static void MapMonitoreoEndpoints(this IEndpointRouteBuilder app)
        {
            // ==================== CERTIFICADOS ====================
            var certificados = app.MapGroup("/api/certificados").WithTags("Certificados");

            certificados.MapPost("/", async (CrearCertificadoRequest request, ICertificadoService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/certificados/{id}", new IdResponse(id));
            });

            certificados.MapGet("/estudiante/{estudianteId:int}", async (int estudianteId, ICertificadoService service) =>
            {
                var lista = await service.ListarPorEstudianteAsync(estudianteId);
                return Results.Ok(lista);
            });

            certificados.MapGet("/validar/{codigoValidacion}", async (string codigoValidacion, ICertificadoService service) =>
            {
                var certificado = await service.ValidarPorCodigoAsync(codigoValidacion);
                return certificado is not null
                    ? Results.Ok(certificado)
                    : Results.NotFound(new MensajeResponse("Certificado no encontrado o código inválido."));
            });

            certificados.MapPut("/{id:int}", async (int id, ActualizarCertificadoRequest request, ICertificadoService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Certificado actualizado correctamente."));
            });

            certificados.MapDelete("/{id:int}", async (int id, ICertificadoService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Certificado eliminado correctamente."));
            });

            // ==================== ESTUDIANTES EN RIESGO ====================
            var riesgo = app.MapGroup("/api/estudiantes-riesgo").WithTags("EstudiantesRiesgo");

            riesgo.MapPost("/", async (CrearEstudianteRiesgoRequest request, IEstudianteRiesgoService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/estudiantes-riesgo/{id}", new IdResponse(id));
            });

            riesgo.MapGet("/", async (int? cursoId, IEstudianteRiesgoService service) =>
            {
                var lista = await service.ListarAsync(cursoId);
                return Results.Ok(lista);
            });

            riesgo.MapPut("/{id:int}", async (int id, ActualizarEstudianteRiesgoRequest request, IEstudianteRiesgoService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Registro de riesgo actualizado correctamente."));
            });

            riesgo.MapDelete("/{id:int}", async (int id, IEstudianteRiesgoService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Registro de riesgo eliminado correctamente."));
            });

            // ==================== PROGRESO DE LECCIONES ====================
            var progreso = app.MapGroup("/api/progreso-lecciones").WithTags("ProgresoLecciones");

            progreso.MapPost("/", async (GuardarProgresoLeccionRequest request, IProgresoLeccionService service) =>
            {
                await service.GuardarAsync(request);
                return Results.Ok(new MensajeResponse("Progreso de lección guardado correctamente."));
            });

            progreso.MapGet("/estudiante/{estudianteId:int}", async (int estudianteId, int? cursoId, IProgresoLeccionService service) =>
            {
                var lista = await service.ListarPorEstudianteAsync(estudianteId, cursoId);
                return Results.Ok(lista);
            });

            progreso.MapDelete("/{id:int}", async (int id, IProgresoLeccionService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Registro de progreso eliminado correctamente."));
            });
        }
    }
}
