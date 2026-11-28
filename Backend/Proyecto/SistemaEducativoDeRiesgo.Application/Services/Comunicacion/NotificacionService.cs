using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Application.Services.Comunicacion
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _repository;

        public NotificacionService(INotificacionRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearNotificacionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                throw new ArgumentException("El título no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(request.Mensaje))
                throw new ArgumentException("El mensaje no puede estar vacío.");

            var notificacion = new Notificacion
            {
                UsuarioId = request.UsuarioId,
                Titulo = request.Titulo.Trim(),
                Mensaje = request.Mensaje.Trim()
            };

            return _repository.InsertarAsync(notificacion);
        }

        public Task MarcarLeidaAsync(int id) => _repository.MarcarLeidaAsync(id);

        public Task MarcarTodasLeidasAsync(int usuarioId) => _repository.MarcarTodasLeidasAsync(usuarioId);

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<NotificacionResponse>> ListarPorUsuarioAsync(int usuarioId)
        {
            var notificaciones = await _repository.ListarPorUsuarioAsync(usuarioId);
            return notificaciones.Select(n => new NotificacionResponse(
                n.Id, n.UsuarioId, n.Titulo, n.Mensaje, n.Leido, n.FechaCreacion
            )).ToList();
        }
    }
}
