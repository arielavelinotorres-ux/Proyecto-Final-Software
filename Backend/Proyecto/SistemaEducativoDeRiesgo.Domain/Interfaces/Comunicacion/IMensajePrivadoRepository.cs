using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion
{
    public interface IMensajePrivadoRepository
    {

        Task<int> InsertarAsync(MensajePrivado mensaje);
        Task MarcarComoLeidoAsync(int id);
        Task EliminarAsync(int id);
        Task<List<MensajePrivado>> ListarRecibidosAsync(int receptorId);
        Task<List<MensajePrivado>> ListarEnviadosAsync(int emisorId);

    }
}
