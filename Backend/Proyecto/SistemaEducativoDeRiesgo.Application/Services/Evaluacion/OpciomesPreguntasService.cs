using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Services.Evaluacion
{
    public class OpcionesPreguntasService : IOpcionesPreguntasService
    {
        private readonly IOpcionesPreguntasRepository _opcionesPreguntasRepository;

        public OpcionesPreguntasService(IOpcionesPreguntasRepository opcionesPreguntasRepository)
        {
            _opcionesPreguntasRepository = opcionesPreguntasRepository;
        }

        public Task<int> CrearAsync(CrearOpcionPreguntaRequest request)
        {
            var opcion = new OpcionPregunta
            {
                PreguntaId = request.PreguntaId,
                TextoOpcion = request.TextoOpcion.Trim(),
                EsCorrecta = request.EsCorrecta ?? false,
                Orden = request.Orden
            };

            return _opcionesPreguntasRepository.InsertarAsync(opcion);
        }

        public Task ActualizarAsync(int id, ActualizarOpcionPreguntaRequest request)
        {
            var opcion = new OpcionPregunta
            {
                Id = id,
                TextoOpcion = request.TextoOpcion.Trim(),
                EsCorrecta = request.EsCorrecta ?? false,
                Orden = request.Orden
            };

            return _opcionesPreguntasRepository.ActualizarAsync(opcion);
        }

        public Task EliminarAsync(int id) => _opcionesPreguntasRepository.EliminarAsync(id);

        public async Task<List<OpcionPreguntaResponse>> ListarAsync()
        {
            var opciones = await _opcionesPreguntasRepository.ListarAsync();
            return opciones.Select(o => new OpcionPreguntaResponse(
                o.Id,
                o.PreguntaId,
                o.TextoOpcion,
                o.EsCorrecta,
                o.Orden
            )).ToList();
        }
    }

}
