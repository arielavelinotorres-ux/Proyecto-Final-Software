using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Services.Evaluacion
{
    public class PreguntasService : IPreguntasService
    {
        private readonly IPreguntasRepository _preguntasRepository;

        public PreguntasService(IPreguntasRepository preguntasRepository)
        {
            _preguntasRepository = preguntasRepository;
        }

        public Task<int> CrearAsync(CrearPreguntaRequest request)
        {
            var pregunta = new Pregunta
            {
                EvaluacionId = request.EvaluacionId,
                Enunciado = request.Enunciado.Trim(),
                TipoPregunta = request.TipoPregunta?.Trim() ?? "OpcionMultiple",
                Puntaje = request.Puntaje ?? 1m,
                Orden = request.Orden
            };

            return _preguntasRepository.InsertarAsync(pregunta);
        }

        public Task ActualizarAsync(int id, ActualizarPreguntaRequest request)
        {
            var pregunta = new Pregunta
            {
                Id = id,
                Enunciado = request.Enunciado.Trim(),
                TipoPregunta = request.TipoPregunta?.Trim() ?? "OpcionMultiple",
                Puntaje = request.Puntaje ?? 1m,
                Orden = request.Orden
            };

            return _preguntasRepository.ActualizarAsync(pregunta);
        }

        public Task EliminarAsync(int id) => _preguntasRepository.EliminarAsync(id);

        public async Task<List<PreguntaResponse>> ListarAsync()
        {
            var preguntas = await _preguntasRepository.ListarAsync();
            return preguntas.Select(p => new PreguntaResponse(
                p.Id,
                p.EvaluacionId,
                p.Enunciado,
                p.TipoPregunta,
                p.Puntaje,
                p.Orden
            )).ToList();
        }
    }
}
