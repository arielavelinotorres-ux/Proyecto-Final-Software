using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;


namespace SistemaEducativoDeRiesgo.Application.Services.Evaluaciones
{
    public class PreguntaService : IPreguntaService
    {
        private static readonly HashSet<string> TiposValidos = new(StringComparer.Ordinal)
        {
            "OpcionMultiple", "VerdaderoFalso", "TextoAbierto"
        };

        private readonly IPreguntaRepository _repository;

        public PreguntaService(IPreguntaRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearPreguntaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Enunciado))
                throw new ArgumentException("El enunciado no puede estar vacío.");

            var tipo = request.TipoPregunta?.Trim() ?? string.Empty;
            if (!TiposValidos.Contains(tipo))
                throw new ArgumentException("Tipo de pregunta no válido.");

            if (request.Puntaje <= 0)
                throw new ArgumentException("El puntaje debe ser mayor a cero.");

            var pregunta = new Pregunta
            {
                EvaluacionId = request.EvaluacionId,
                Enunciado = request.Enunciado.Trim(),
                TipoPregunta = tipo,
                Puntaje = request.Puntaje,
                Orden = request.Orden
            };

            return _repository.InsertarAsync(pregunta);
        }

        public Task ActualizarAsync(int id, ActualizarPreguntaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Enunciado))
                throw new ArgumentException("El enunciado no puede estar vacío.");

            var tipo = request.TipoPregunta?.Trim() ?? string.Empty;
            if (!TiposValidos.Contains(tipo))
                throw new ArgumentException("Tipo de pregunta no válido.");

            if (request.Puntaje <= 0)
                throw new ArgumentException("El puntaje debe ser mayor a cero.");

            var pregunta = new Pregunta
            {
                Id = id,
                Enunciado = request.Enunciado.Trim(),
                TipoPregunta = tipo,
                Puntaje = request.Puntaje,
                Orden = request.Orden
            };

            return _repository.ActualizarAsync(pregunta);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<PreguntaResponse>> ListarPorEvaluacionAsync(int evaluacionId)
        {
            var preguntas = await _repository.ListarPorEvaluacionAsync(evaluacionId);
            return preguntas.Select(p => new PreguntaResponse(
                p.Id, p.EvaluacionId, p.Enunciado, p.TipoPregunta, p.Puntaje, p.Orden
            )).ToList();
        }
    }
}
