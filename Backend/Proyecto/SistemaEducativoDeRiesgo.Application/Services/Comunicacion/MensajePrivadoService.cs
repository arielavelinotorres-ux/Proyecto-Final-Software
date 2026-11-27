using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Application.Services.Comunicacion
{
    public class MensajePrivadoService : IMensajePrivadoService
    {
        private readonly IMensajePrivadoRepository _repository;

        public MensajePrivadoService(IMensajePrivadoRepository repository)
        {
            _repository = repository;
        }

        public Task<int> EnviarAsync(CrearMensajePrivadoRequest request)
        {
            if (request.EmisorId == request.ReceptorId)
                throw new ArgumentException("No puedes enviarte un mensaje a ti mismo.");

            if (string.IsNullOrWhiteSpace(request.Contenido))
                throw new ArgumentException("El contenido del mensaje no puede estar vacío.");

            var mensaje = new MensajePrivado
            {
                EmisorId = request.EmisorId,
                ReceptorId = request.ReceptorId,
                Asunto = request.Asunto?.Trim(),
                Contenido = request.Contenido.Trim()
            };

            return _repository.InsertarAsync(mensaje);
        }

        public Task MarcarComoLeidoAsync(int id) => _repository.MarcarComoLeidoAsync(id);

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<MensajeRecibidoResponse>> ListarRecibidosAsync(int receptorId)
        {
            var mensajes = await _repository.ListarRecibidosAsync(receptorId);
            return mensajes.Select(m => new MensajeRecibidoResponse(
                m.Id,
                m.EmisorId,
                m.EmisorNombre ?? string.Empty,
                m.ReceptorId,
                m.Asunto,
                m.Contenido,
                m.Leido,
                m.FechaEnvio
            )).ToList();
        }

        public async Task<List<MensajeEnviadoResponse>> ListarEnviadosAsync(int emisorId)
        {
            var mensajes = await _repository.ListarEnviadosAsync(emisorId);
            return mensajes.Select(m => new MensajeEnviadoResponse(
                m.Id,
                m.EmisorId,
                m.ReceptorId,
                m.ReceptorNombre ?? string.Empty,
                m.Asunto,
                m.Contenido,
                m.Leido,
                m.FechaEnvio
            )).ToList();
        }
    }
}
