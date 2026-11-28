using SistemaEducativoDeRiesgo.Application.DTOs;
using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones;

namespace SistemaEducativoDeRiesgo.Web.Api.Endpoints
{
    public static class EvaluacionesEndpoints
    {
        public static void MapEvaluacionesEndpoints(this IEndpointRouteBuilder app)
        {
            // ==================== EVALUACIONES ====================
            var evaluaciones = app.MapGroup("/api/evaluaciones").WithTags("Evaluaciones");

            evaluaciones.MapPost("/", async (CrearEvaluacionRequest request, IEvaluacionService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/evaluaciones/{id}", new IdResponse(id));
            });

            evaluaciones.MapGet("/curso/{cursoId:int}", async (int cursoId, IEvaluacionService service) =>
            {
                var lista = await service.ListarPorCursoAsync(cursoId);
                return Results.Ok(lista);
            });

            evaluaciones.MapPut("/{id:int}", async (int id, ActualizarEvaluacionRequest request, IEvaluacionService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Evaluación actualizada correctamente."));
            });

            evaluaciones.MapPatch("/{id:int}/archivar", async (int id, IEvaluacionService service) =>
            {
                await service.DesactivarAsync(id);
                return Results.Ok(new MensajeResponse("Evaluación archivada correctamente."));
            });

            // ==================== ENTREGAS DE TAREAS ====================
            var entregas = app.MapGroup("/api/entregas-tareas").WithTags("EntregasTareas");

            entregas.MapPost("/", async (CrearEntregaTareaRequest request, IEntregaTareaService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/entregas-tareas/{id}", new IdResponse(id));
            });

            entregas.MapGet("/evaluacion/{evaluacionId:int}", async (int evaluacionId, IEntregaTareaService service) =>
            {
                var lista = await service.ListarPorEvaluacionAsync(evaluacionId);
                return Results.Ok(lista);
            });

            entregas.MapPut("/{id:int}", async (int id, ActualizarEntregaTareaRequest request, IEntregaTareaService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Entrega calificada/actualizada correctamente."));
            });

            entregas.MapDelete("/{id:int}", async (int id, IEntregaTareaService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Entrega eliminada correctamente."));
            });

            // ==================== PREGUNTAS ====================
            var preguntas = app.MapGroup("/api/preguntas").WithTags("Preguntas");

            preguntas.MapPost("/", async (CrearPreguntaRequest request, IPreguntaService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/preguntas/{id}", new IdResponse(id));
            });

            preguntas.MapGet("/evaluacion/{evaluacionId:int}", async (int evaluacionId, IPreguntaService service) =>
            {
                var lista = await service.ListarPorEvaluacionAsync(evaluacionId);
                return Results.Ok(lista);
            });

            preguntas.MapPut("/{id:int}", async (int id, ActualizarPreguntaRequest request, IPreguntaService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Pregunta actualizada correctamente."));
            });

            preguntas.MapDelete("/{id:int}", async (int id, IPreguntaService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Pregunta eliminada correctamente."));
            });

            // ==================== OPCIONES DE PREGUNTA ====================
            var opciones = app.MapGroup("/api/opciones-pregunta").WithTags("OpcionesPregunta");

            opciones.MapPost("/", async (CrearOpcionPreguntaRequest request, IOpcionPreguntaService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/opciones-pregunta/{id}", new IdResponse(id));
            });

            opciones.MapGet("/pregunta/{preguntaId:int}", async (int preguntaId, IOpcionPreguntaService service) =>
            {
                var lista = await service.ListarPorPreguntaAsync(preguntaId);
                return Results.Ok(lista);
            });

            opciones.MapPut("/{id:int}", async (int id, ActualizarOpcionPreguntaRequest request, IOpcionPreguntaService service) =>
            {
                await service.ActualizarAsync(id, request);
                return Results.Ok(new MensajeResponse("Opción actualizada correctamente."));
            });

            opciones.MapDelete("/{id:int}", async (int id, IOpcionPreguntaService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Opción eliminada correctamente."));
            });

            // ==================== RESPUESTAS DE ESTUDIANTE ====================
            var respuestas = app.MapGroup("/api/respuestas-estudiante").WithTags("RespuestasEstudiante");

            respuestas.MapPost("/", async (CrearRespuestaEstudianteRequest request, IRespuestaEstudianteService service) =>
            {
                var id = await service.CrearAsync(request);
                return Results.Created($"/api/respuestas-estudiante/{id}", new IdResponse(id));
            });

            respuestas.MapGet("/estudiante/{estudianteId:int}", async (int estudianteId, int? evaluacionId, IRespuestaEstudianteService service) =>
            {
                var lista = await service.ListarPorEstudianteAsync(estudianteId, evaluacionId);
                return Results.Ok(lista);
            });

            respuestas.MapPatch("/{id:int}/calificar", async (int id, CalificarRespuestaEstudianteRequest request, IRespuestaEstudianteService service) =>
            {
                await service.CalificarAsync(id, request);
                return Results.Ok(new MensajeResponse("Respuesta calificada correctamente."));
            });

            respuestas.MapDelete("/{id:int}", async (int id, IRespuestaEstudianteService service) =>
            {
                await service.EliminarAsync(id);
                return Results.Ok(new MensajeResponse("Respuesta eliminada correctamente."));
            });
        }
    }
}
