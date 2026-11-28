using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Services.Evaluacion
{
    public class EntregaTareasService : IEntregaTareasService
    {
        private readonly IEntregaTareasRepository _entregaTareasRepository;
    }

    public interface IEntregaTareasService
    {
        private readonly IEntregaTareaRepository _repository;

        public EntregaTareasService(IEntregaTareaRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearEntregaTareaRequest request)
        {
            var entrega = new EntregaTarea
            {
                EvaluacionId = request.EvaluacionId,
                EstudianteId = request.EstudianteId,
                ArchivoUrl = request.ArchivoUrl?.Trim(),
                ContenidoTexto = request.ContenidoTexto?.Trim(),
                Calificacion = request.Calificacion,
                Retroalimentacion = request.Retroalimentacion?.Trim(),
                Estado = request.Estado?.Trim() ?? "Enviado"
            };

            return _repository.InsertarAsync(entrega);
        }

        public Task ActualizarAsync(int id, ActualizarEntregaTareaRequest request)
        {
            var entrega = new EntregaTarea
            {
                Id = id,
                ArchivoUrl = request.ArchivoUrl?.Trim(),
                ContenidoTexto = request.ContenidoTexto?.Trim(),
                Calificacion = request.Calificacion,
                Retroalimentacion = request.Retroalimentacion?.Trim(),
                Estado = request.Estado?.Trim() ?? "Enviado"
            };

            return _repository.ActualizarAsync(entrega);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<EntregaTareaResponse>> ListarAsync()
        {
            var entregas = await _repository.ListarAsync();
            return entregas.Select(e => new EntregaTareaResponse(
                e.Id,
                e.EvaluacionId,
                e.EstudianteId,
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
