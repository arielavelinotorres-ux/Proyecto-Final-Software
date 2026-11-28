using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;

namespace SistemaEducativoDeRiesgo.Application.Services.Evaluaciones
{
    public class EntregaTareaService : IEntregaTareaService
    {
        private static readonly HashSet<string> EstadosValidos = new(StringComparer.Ordinal)
        {
            "Enviado", "Calificado", "Revisión"
        };

        private readonly IEntregaTareaRepository _repository;

        public EntregaTareaService(IEntregaTareaRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearEntregaTareaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ArchivoUrl) && string.IsNullOrWhiteSpace(request.ContenidoTexto))
                throw new ArgumentException("Debe enviar un archivo o proporcionar contenido de texto.");

            var entrega = new EntregaTarea
            {
                EvaluacionId = request.EvaluacionId,
                EstudianteId = request.EstudianteId,
                ArchivoUrl = string.IsNullOrWhiteSpace(request.ArchivoUrl) ? null : request.ArchivoUrl.Trim(),
                ContenidoTexto = string.IsNullOrWhiteSpace(request.ContenidoTexto) ? null : request.ContenidoTexto.Trim()
            };

            return _repository.InsertarAsync(entrega);
        }

        public Task ActualizarAsync(int id, ActualizarEntregaTareaRequest request)
        {
            if (request.Calificacion < 0)
                throw new ArgumentException("La calificación no puede ser negativa.");

            var estado = request.Estado?.Trim() ?? string.Empty;
            if (!EstadosValidos.Contains(estado))
                throw new ArgumentException("Estado inválido. Solo se permite Enviado, Calificado o Revisión.");

            return _repository.ActualizarCalificacionAsync(
                id,
                request.Calificacion,
                request.Retroalimentacion?.Trim(),
                estado);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<EntregaTareaResponse>> ListarPorEvaluacionAsync(int evaluacionId)
        {
            var entregas = await _repository.ListarPorEvaluacionAsync(evaluacionId);
            return entregas.Select(e => new EntregaTareaResponse(
                e.Id,
                e.EvaluacionId,
                e.EstudianteId,
                e.EstudianteNombre ?? string.Empty,
                e.ArchivoUrl,
                e.ContenidoTexto,
                e.FechaEntrega,
                e.Calificacion,
                e.Retroalimentacion,
                e.Estado
            )).ToList();
        }
    }
}
