using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;

namespace SistemaEducativoDeRiesgo.Application.Services.Evaluaciones
{
    public class EvaluacionService : IEvaluacionService
    {
        private static readonly HashSet<string> TiposValidos = new(StringComparer.Ordinal)
        {
            "Tarea", "Examen", "Cuestionario", "Proyecto"
        };

        private static readonly HashSet<string> EstadosValidos = new(StringComparer.Ordinal)
        {
            "Activo", "Cerrado", "Archivado"
        };

        private readonly IEvaluacionRepository _repository;

        public EvaluacionService(IEvaluacionRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearEvaluacionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                throw new ArgumentException("El título de la evaluación no puede estar vacío.");

            var tipo = request.Tipo?.Trim() ?? string.Empty;
            if (!TiposValidos.Contains(tipo))
                throw new ArgumentException("Tipo de evaluación no válido.");

            if (request.PuntajeMaximo <= 0)
                throw new ArgumentException("El puntaje máximo debe ser mayor a cero.");

            if (request.FechaInicio.HasValue && request.FechaLimite.HasValue && request.FechaLimite < request.FechaInicio)
                throw new ArgumentException("La fecha límite no puede ser anterior a la fecha de inicio.");

            var evaluacion = new Evaluacion
            {
                CursoId = request.CursoId,
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                Tipo = tipo,
                PuntajeMaximo = request.PuntajeMaximo,
                FechaInicio = request.FechaInicio ?? DateTime.Now,
                FechaLimite = request.FechaLimite
            };

            return _repository.InsertarAsync(evaluacion);
        }

        public Task ActualizarAsync(int id, ActualizarEvaluacionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                throw new ArgumentException("El título de la evaluación no puede estar vacío.");

            var tipo = request.Tipo?.Trim() ?? string.Empty;
            if (!TiposValidos.Contains(tipo))
                throw new ArgumentException("Tipo de evaluación no válido.");

            if (request.PuntajeMaximo <= 0)
                throw new ArgumentException("El puntaje máximo debe ser mayor a cero.");

            var estado = request.Estado?.Trim() ?? string.Empty;
            if (!EstadosValidos.Contains(estado))
                throw new ArgumentException("Estado inválido. Solo se permite Activo, Cerrado o Archivado.");

            if (request.FechaInicio.HasValue && request.FechaLimite.HasValue && request.FechaLimite < request.FechaInicio)
                throw new ArgumentException("La fecha límite no puede ser anterior a la fecha de inicio.");

            var evaluacion = new Evaluacion
            {
                Id = id,
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                Tipo = tipo,
                PuntajeMaximo = request.PuntajeMaximo,
                FechaInicio = request.FechaInicio ?? default,
                FechaLimite = request.FechaLimite,
                Estado = estado
            };

            return _repository.ActualizarAsync(evaluacion);
        }

        public Task DesactivarAsync(int id) => _repository.DesactivarAsync(id);

        public async Task<List<EvaluacionResponse>> ListarPorCursoAsync(int cursoId)
        {
            var lista = await _repository.ListarPorCursoAsync(cursoId);
            return lista.Select(e => new EvaluacionResponse(
                e.Id, e.CursoId, e.Titulo, e.Descripcion, e.Tipo,
                e.PuntajeMaximo, e.FechaInicio, e.FechaLimite, e.Estado, e.CreadoEn
            )).ToList();
        }
    }
}
