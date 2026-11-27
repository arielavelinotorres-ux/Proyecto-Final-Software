using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion
{
    public interface INotificacionService
    {
        Task<int> CrearAsync(CrearNotificacionRequest request);
        Task MarcarLeidaAsync(int id);
        Task MarcarTodasLeidasAsync(int usuarioId);
        Task EliminarAsync(int id);
        Task<List<NotificacionResponse>> ListarPorUsuarioAsync(int usuarioId);
    }
}
