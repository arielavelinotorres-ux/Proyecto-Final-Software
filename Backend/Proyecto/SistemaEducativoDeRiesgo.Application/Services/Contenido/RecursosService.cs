using SistemaEducativoDeRiesgo.Application.Interfaces.Contenido;
using System;
using System.Collections.Generic;
using System.Text;
using static SistemaEducativoDeRiesgo.Application.DTOs.Contenido.RecursosDtos;

namespace SistemaEducativoDeRiesgo.Application.Services.Contenido
{
    public class RecursosService : IRecursosService
    {
        private readonly IRecursosRepository _recursosRepository;

        public RecursosService(IRecursosRepository recursosRepository)
        {
            _recursosRepository = recursosRepository;
        }

        public Task<int> CrearAsync(CrearRecursoRequest request)
        {
            var recurso = new Recurso
            {
                LeccionId = request.LeccionId,
                Nombre = request.Nombre.Trim(),
                TipoArchivo = request.TipoArchivo?.Trim(),
                UrlRecurso = request.UrlRecurso.Trim()
            };

            return _recursosRepository.InsertarAsync(recurso);
        }

        public Task ActualizarAsync(int id, ActualizarRecursoRequest request)
        {
            var recurso = new Recurso
            {
                Id = id,
                Nombre = request.Nombre.Trim(),
                TipoArchivo = request.TipoArchivo?.Trim(),
                UrlRecurso = request.UrlRecurso.Trim()
            };

            return _recursosRepository.ActualizarAsync(recurso);
        }

        public Task EliminarAsync(int id) => _recursosRepository.EliminarAsync(id);

        public async Task<List<RecursoResponse>> ListarAsync()
        {
            var recursos = await _recursosRepository.ListarAsync();
            return recursos.Select(r => new RecursoResponse(
                r.Id,
                r.LeccionId,
                r.Nombre,
                r.TipoArchivo,
                r.UrlRecurso,
                r.CreadoEn
            )).ToList();
        }
    }
}
