using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion
{
    public interface IMensajePrivadoService
    {
        Task<int> EnviarAsync(CrearMensajePrivadoRequest request);
        Task MarcarComoLeidoAsync(int id);
        Task EliminarAsync(int id);
        Task<List<MensajeRecibidoResponse>> ListarRecibidosAsync(int receptorId);
        Task<List<MensajeEnviadoResponse>> ListarEnviadosAsync(int emisorId);
    }
}
