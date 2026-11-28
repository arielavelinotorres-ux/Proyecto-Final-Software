using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;


namespace SistemaEducativoDeRiesgo.Application.Services.Evaluaciones
{
    public class OpcionPreguntaService : IOpcionPreguntaService
    {
        private readonly IOpcionPreguntaRepository _repository;

        public OpcionPreguntaService(IOpcionPreguntaRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearOpcionPreguntaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TextoOpcion))
                throw new ArgumentException("El texto de la opción no puede estar vacío.");

            var opcion = new OpcionPregunta
            {
                PreguntaId = request.PreguntaId,
                TextoOpcion = request.TextoOpcion.Trim(),
                EsCorrecta = request.EsCorrecta,
                Orden = request.Orden
            };

            return _repository.InsertarAsync(opcion);
        }

        public Task ActualizarAsync(int id, ActualizarOpcionPreguntaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TextoOpcion))
                throw new ArgumentException("El texto de la opción no puede estar vacío.");

            var opcion = new OpcionPregunta
            {
                Id = id,
                TextoOpcion = request.TextoOpcion.Trim(),
                EsCorrecta = request.EsCorrecta,
                Orden = request.Orden
            };

            return _repository.ActualizarAsync(opcion);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<OpcionPreguntaResponse>> ListarPorPreguntaAsync(int preguntaId)
        {
            var opciones = await _repository.ListarPorPreguntaAsync(preguntaId);
            return opciones.Select(o => new OpcionPreguntaResponse(
                o.Id, o.PreguntaId, o.TextoOpcion, o.EsCorrecta, o.Orden
            )).ToList();
        }
    }
}
