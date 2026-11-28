using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo;
using SistemaEducativoDeRiesgo.Application.Interfaces.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo;


namespace SistemaEducativoDeRiesgo.Application.Services.Monitoreo
{
    public class ProgresoLeccionService : IProgresoLeccionService
    {
        private readonly IProgresoLeccionRepository _repository;

        public ProgresoLeccionService(IProgresoLeccionRepository repository)
        {
            _repository = repository;
        }

        public Task GuardarAsync(GuardarProgresoLeccionRequest request)
        {
            return _repository.GuardarAsync(request.EstudianteId, request.LeccionId, request.Completado);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<ProgresoLeccionResponse>> ListarPorEstudianteAsync(int estudianteId, int? cursoId)
        {
            var lista = await _repository.ListarPorEstudianteAsync(estudianteId, cursoId);
            return lista.Select(p => new ProgresoLeccionResponse(
                p.Id,
                p.EstudianteId,
                p.LeccionId,
                p.LeccionTitulo ?? string.Empty,
                p.CursoId,
                p.Completado,
                p.FechaCompletado
            )).ToList();
        }
    }
}
