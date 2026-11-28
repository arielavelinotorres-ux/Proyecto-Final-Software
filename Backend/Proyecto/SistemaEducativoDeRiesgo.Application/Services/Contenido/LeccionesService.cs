using System;
using System.Collections.Generic;
using System.Text;
using static SistemaEducativoDeRiesgo.Application.DTOs.Contenido.LeccionesDtos;

namespace SistemaEducativoDeRiesgo.Application.Services.Contenido
{
    public class LeccionesService : ILeccionesService
    {
        private readonly ILeccionesRepository _leccionesRepository;

        public LeccionesService(ILeccionesRepository leccionesRepository)
        {
            _leccionesRepository = leccionesRepository;
        }

        public Task<int> CrearAsync(CrearLeccionRequest request)
        {
            var leccion = new Leccion
            {
                SeccionId = request.SeccionId,
                Titulo = request.Titulo.Trim(),
                ContenidoTexto = request.ContenidoTexto?.Trim(),
                UrlVideo = request.UrlVideo?.Trim(),
                DuracionMinutos = request.DuracionMinutos ?? 0,
                Orden = request.Orden,
                EsGratuita = request.EsGratuita ?? false
            };

            return _leccionesRepository.InsertarAsync(leccion);
        }

        public Task ActualizarAsync(int id, ActualizarLeccionRequest request)
        {
            var leccion = new Leccion
            {
                Id = id,
                Titulo = request.Titulo.Trim(),
                ContenidoTexto = request.ContenidoTexto?.Trim(),
                UrlVideo = request.UrlVideo?.Trim(),
                DuracionMinutos = request.DuracionMinutos ?? 0,
                Orden = request.Orden,
                EsGratuita = request.EsGratuita ?? false
            };

            return _leccionesRepository.ActualizarAsync(leccion);
        }

        public Task EliminarAsync(int id) => _leccionesRepository.EliminarAsync(id);

        public async Task<List<LeccionResponse>> ListarAsync()
        {
            var lecciones = await _leccionesRepository.ListarAsync();
            return lecciones.Select(l => new LeccionResponse(
                l.Id,
                l.SeccionId,
                l.Titulo,
                l.ContenidoTexto,
                l.UrlVideo,
                l.DuracionMinutos,
                l.Orden,
                l.EsGratuita,
                l.CreadoEn
            )).ToList();
        }
    }
}
