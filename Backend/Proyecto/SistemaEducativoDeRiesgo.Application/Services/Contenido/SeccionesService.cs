using SistemaEducativoDeRiesgo.Application.Interfaces.Contenido;
using System;
using System.Collections.Generic;
using System.Text;
using static SistemaEducativoDeRiesgo.Application.DTOs.Contenido.SeccionesDtos;

namespace SistemaEducativoDeRiesgo.Application.Services.Contenido
{
    public class SeccionesService : ISeccionesService
    {
        private readonly ISeccionesRepository _seccionesRepository;

        public SeccionesService(ISeccionesRepository seccionesRepository)
        {
            _seccionesRepository = seccionesRepository;
        }

        public Task<int> CrearAsync(CrearSeccionRequest request)
        {
            var seccion = new Seccion
            {
                CursoId = request.CursoId,
                Titulo = request.Titulo.Trim(),
                Orden = request.Orden
            };

            return _seccionesRepository.InsertarAsync(seccion);
        }

        public Task ActualizarAsync(int id, ActualizarSeccionRequest request)
        {
            var seccion = new Seccion
            {
                Id = id,
                Titulo = request.Titulo.Trim(),
                Orden = request.Orden
            };

            return _seccionesRepository.ActualizarAsync(seccion);
        }

        public Task EliminarAsync(int id) => _seccionesRepository.EliminarAsync(id);

        public async Task<List<SeccionResponse>> ListarAsync()
        {
            var secciones = await _seccionesRepository.ListarAsync();
            return secciones.Select(s => new SeccionResponse(
                s.Id,
                s.CursoId,
                s.Titulo,
                s.Orden,
                s.CreadoEn
            )).ToList();
        }
    }
}
