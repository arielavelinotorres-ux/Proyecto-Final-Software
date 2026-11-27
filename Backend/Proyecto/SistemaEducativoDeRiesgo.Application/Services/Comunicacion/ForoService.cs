using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Application.Services.Comunicacion
{
    public class ForoService : IForoService
    {
        private static readonly HashSet<string> EstadosValidos = new(StringComparer.Ordinal)
        {
            "Abierto", "Cerrado", "Archivado"
        };

        private readonly IForoRepository _repository;

        public ForoService(IForoRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearForoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                throw new ArgumentException("El título del foro no puede estar vacío.");

            var foro = new Foro
            {
                CursoId = request.CursoId,
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion?.Trim()
            };

            return _repository.InsertarAsync(foro);
        }

        public Task ActualizarAsync(int id, ActualizarForoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                throw new ArgumentException("El título del foro no puede estar vacío.");

            var estado = request.Estado?.Trim() ?? string.Empty;
            if (!EstadosValidos.Contains(estado))
                throw new ArgumentException("Estado inválido. Solo se permite Abierto, Cerrado o Archivado.");

            var foro = new Foro
            {
                Id = id,
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                Estado = estado
            };

            return _repository.ActualizarAsync(foro);
        }

        public Task DesactivarAsync(int id) => _repository.DesactivarAsync(id);

        public async Task<List<ForoResponse>> ListarPorCursoAsync(int cursoId)
        {
            var foros = await _repository.ListarPorCursoAsync(cursoId);
            return foros.Select(f => new ForoResponse(
                f.Id, f.CursoId, f.Titulo, f.Descripcion, f.Estado, f.CreadoEn
            )).ToList();
        }
    }
}
