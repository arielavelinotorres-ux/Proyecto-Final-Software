using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;


namespace SistemaEducativoDeRiesgo.Application.Services.Evaluaciones
{
    public class RespuestaEstudianteService : IRespuestaEstudianteService
    {
        private readonly IRespuestaEstudianteRepository _repository;

        public RespuestaEstudianteService(IRespuestaEstudianteRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearRespuestaEstudianteRequest request)
        {
            if (!request.OpcionSeleccionadaId.HasValue && string.IsNullOrWhiteSpace(request.RespuestaTexto))
                throw new ArgumentException("Debe seleccionar una opción o escribir una respuesta.");

            var respuesta = new RespuestaEstudiante
            {
                EstudianteId = request.EstudianteId,
                PreguntaId = request.PreguntaId,
                OpcionSeleccionadaId = request.OpcionSeleccionadaId,
                RespuestaTexto = string.IsNullOrWhiteSpace(request.RespuestaTexto) ? null : request.RespuestaTexto.Trim(),
                EsCorrecta = request.EsCorrecta,
                PuntajeObtenido = request.PuntajeObtenido
            };

            return _repository.InsertarAsync(respuesta);
        }

        public Task CalificarAsync(int id, CalificarRespuestaEstudianteRequest request)
        {
            if (request.PuntajeObtenido < 0)
                throw new ArgumentException("El puntaje obtenido no puede ser negativo.");

            return _repository.CalificarAsync(id, request.EsCorrecta, request.PuntajeObtenido);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<RespuestaEstudianteResponse>> ListarPorEstudianteAsync(int estudianteId, int? evaluacionId)
        {
            var respuestas = await _repository.ListarPorEstudianteAsync(estudianteId, evaluacionId);
            return respuestas.Select(r => new RespuestaEstudianteResponse(
                r.Id,
                r.EstudianteId,
                r.PreguntaId,
                r.OpcionSeleccionadaId,
                r.RespuestaTexto,
                r.EsCorrecta,
                r.PuntajeObtenido,
                r.FechaRespuesta
            )).ToList();
        }
    }
}
