using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Services.Evaluacion
{

    public class PreguntasEstudiantesService : IPreguntasEstudiantesService
    {
        private readonly IPreguntasEstudiantesRepository _preguntasEstudiantesRepository;

        public PreguntasEstudiantesService(IPreguntasEstudiantesRepository preguntasEstudiantesRepository)
        {
            _preguntasEstudiantesRepository = preguntasEstudiantesRepository;
        }

        public Task<int> CrearAsync(CrearPreguntaEstudianteRequest request)
        {
            var pregunta = new PreguntaEstudiante
            {
                IntentoId = request.IntentoId,
                PreguntaId = request.PreguntaId,
                OpcionSeleccionadaId = request.OpcionSeleccionadaId,
                RespuestaTexto = request.RespuestaTexto?.Trim(),
                EsCorrecta = request.EsCorrecta,
                PuntajeObtenido = request.PuntajeObtenido
            };

            return _preguntasEstudiantesRepository.InsertarAsync(pregunta);
        }

        public Task ActualizarAsync(int id, ActualizarPreguntaEstudianteRequest request)
        {
            var pregunta = new PreguntaEstudiante
            {
                Id = id,
                OpcionSeleccionadaId = request.OpcionSeleccionadaId,
                RespuestaTexto = request.RespuestaTexto?.Trim(),
                EsCorrecta = request.EsCorrecta,
                PuntajeObtenido = request.PuntajeObtenido
            };

            return _preguntasEstudiantesRepository.ActualizarAsync(pregunta);
        }

        public Task EliminarAsync(int id) => _preguntasEstudiantesRepository.EliminarAsync(id);

        public async Task<List<PreguntaEstudianteResponse>> ListarAsync()
        {
            var preguntas = await _preguntasEstudiantesRepository.ListarAsync();
            return preguntas.Select(p => new PreguntaEstudianteResponse(
                p.Id,
                p.IntentoId,
                p.PreguntaId,
                p.OpcionSeleccionadaId,
                p.RespuestaTexto,
                p.EsCorrecta,
                p.PuntajeObtenido
            )).ToList();
        }

    }
}

