using SistemaEducativoDeRiesgo.Application.DTOs.Configuracion;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Configuracion
{
    public interface IFAuditoriaConfiguracion
    {
        Task<int> CrearAsync(CrearAuditoriaConfiguracionRequest request);
        Task ActualizarAsync(int id, ActualizarAuditoriaConfiguracionRequest request);
        Task EliminarAsync(int id);
        Task<List<AuditoriaConfiguracionResponse>> ListarAsync();
    }
}
