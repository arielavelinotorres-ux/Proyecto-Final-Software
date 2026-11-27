using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Application.Services.Comunicacion
{
    public class PublicacionForoService : IPublicacionForoService
    {
        private readonly IPublicacionForoRepository _repository;

        public PublicacionForoService(IPublicacionForoRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearPublicacionForoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Contenido))
                throw new ArgumentException("El contenido de la publicación no puede estar vacío.");

            var publicacion = new PublicacionForo
            {
                ForoId = request.ForoId,
                UsuarioId = request.UsuarioId,
                PublicacionPadreId = request.PublicacionPadreId,
                Contenido = request.Contenido.Trim()
            };

            return _repository.InsertarAsync(publicacion);
        }

        public Task ActualizarAsync(int id, ActualizarPublicacionForoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Contenido))
                throw new ArgumentException("El contenido no puede estar vacío.");

            return _repository.ActualizarAsync(id, request.Contenido.Trim());
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<PublicacionForoResponse>> ListarPorForoAsync(int foroId)
        {
            var publicaciones = await _repository.ListarPorForoAsync(foroId);
            return publicaciones.Select(p => new PublicacionForoResponse(
                p.Id,
                p.ForoId,
                p.UsuarioId,
                p.AutorRol ?? string.Empty,
                p.AutorNombre ?? string.Empty,
                p.PublicacionPadreId,
                p.Contenido,
                p.CreadoEn
            )).ToList();
        }
    }
}
