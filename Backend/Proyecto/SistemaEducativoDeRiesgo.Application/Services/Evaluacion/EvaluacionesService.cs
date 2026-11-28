using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Services.Evaluacion
{
    public class EvaluacionesService : IEvaluacionesService
    {
        private readonly IEvaluacionesRepository _evaluacionesRepository;

        public EvaluacionesService(IEvaluacionesRepository evaluacionesRepository)
        {
            _evaluacionesRepository = evaluacionesRepository;
        }

        public Task<int> CrearAsync(CrearEvaluacionRequest request)
        {
            var evaluacion = new Evaluacion
            {
                CursoId = request.CursoId,
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                Tipo = request.Tipo?.Trim() ?? "Tarea",
                PuntajeMaximo = request.PuntajeMaximo ?? 100m,
                FechaLimite = request.FechaLimite,
                Activo = request.Activo ?? true
            };

            return _evaluacionesRepository.InsertarAsync(evaluacion);
        }

        public Task ActualizarAsync(int id, ActualizarEvaluacionRequest request)
        {
            var evaluacion = new Evaluacion
            {
                Id = id,
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                Tipo = request.Tipo?.Trim() ?? "Tarea",
                PuntajeMaximo = request.PuntajeMaximo ?? 100m,
                FechaLimite = request.FechaLimite,
                Activo = request.Activo ?? true
            };

            return _evaluacionesRepository.ActualizarAsync(evaluacion);
        }

        public Task EliminarAsync(int id) => _evaluacionesRepository.EliminarAsync(id);

        public async Task<List<EvaluacionResponse>> ListarAsync()
        {
            var evaluaciones = await _evaluacionesRepository.ListarAsync();
            return evaluaciones.Select(e => new EvaluacionResponse(
                e.Id,
                e.CursoId,
                e.Titulo,
                e.Descripcion,
                e.Tipo,
                e.PuntajeMaximo,
                e.FechaLimite,
                e.Activo,
                e.CreadoEn
            )).ToList();
        }


    }
}