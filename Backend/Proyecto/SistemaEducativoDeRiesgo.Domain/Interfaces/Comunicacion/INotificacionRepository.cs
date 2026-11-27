using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion
{
    public interface INotificacionRepository
    {
        Task<int> InsertarAsync(Notificacion notificacion);
        Task MarcarLeidaAsync(int id);
        Task MarcarTodasLeidasAsync(int usuarioId);
        Task EliminarAsync(int id);
        Task<List<Notificacion>> ListarPorUsuarioAsync(int usuarioId);
    }
}
