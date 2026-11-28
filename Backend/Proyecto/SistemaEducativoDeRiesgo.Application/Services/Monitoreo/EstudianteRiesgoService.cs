using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo;
using SistemaEducativoDeRiesgo.Application.Interfaces.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo;


namespace SistemaEducativoDeRiesgo.Application.Services.Monitoreo
{
    public class EstudianteRiesgoService : IEstudianteRiesgoService
    {
        private static readonly HashSet<string> NivelesValidos = new(StringComparer.Ordinal)
        {
            "Bajo", "Medio", "Alto", "Critico"
        };

        private readonly IEstudianteRiesgoRepository _repository;

        public EstudianteRiesgoService(IEstudianteRiesgoRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearEstudianteRiesgoRequest request)
        {
            var nivel = request.NivelRiesgo?.Trim() ?? string.Empty;
            if (!NivelesValidos.Contains(nivel))
                throw new ArgumentException("Nivel de riesgo inválido. Solo se permite Bajo, Medio, Alto o Critico.");

            if (string.IsNullOrWhiteSpace(request.Motivo))
                throw new ArgumentException("El motivo es obligatorio.");

            var registro = new EstudianteRiesgo
            {
                EstudianteId = request.EstudianteId,
                CursoId = request.CursoId,
                NivelRiesgo = nivel,
                Motivo = request.Motivo.Trim()
            };

            return _repository.InsertarAsync(registro);
        }

        public Task ActualizarAsync(int id, ActualizarEstudianteRiesgoRequest request)
        {
            var nivel = request.NivelRiesgo?.Trim() ?? string.Empty;
            if (!NivelesValidos.Contains(nivel))
                throw new ArgumentException("Nivel de riesgo inválido. Solo se permite Bajo, Medio, Alto o Critico.");

            if (string.IsNullOrWhiteSpace(request.Motivo))
                throw new ArgumentException("El motivo es obligatorio.");

            var registro = new EstudianteRiesgo
            {
                Id = id,
                NivelRiesgo = nivel,
                Motivo = request.Motivo.Trim(),
                Atendido = request.Atendido
            };

            return _repository.ActualizarAsync(registro);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<EstudianteRiesgoResponse>> ListarAsync(int? cursoId)
        {
            var lista = await _repository.ListarAsync(cursoId);
            return lista.Select(r => new EstudianteRiesgoResponse(
                r.Id,
                r.EstudianteId,
                r.EstudianteNombre ?? string.Empty,
                r.CursoId,
                r.CursoTitulo ?? string.Empty,
                r.NivelRiesgo,
                r.Motivo,
                r.DetectadoEn,
                r.Atendido
            )).ToList();
        }
    }
}
