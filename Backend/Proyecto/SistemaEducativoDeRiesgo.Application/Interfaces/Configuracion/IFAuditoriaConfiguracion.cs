using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Configuracion
{
    internal class IFAuditoriaConfiguracion
    {
        Task<int> CrearAsync(CrearAuditoriaConfiguracionRequest request);

        Task ActualizarAsync(int id, ActualizarAuditoriaConfiguracionRequest request);

        Task EliminarAsync(int id);

        Task<List<AuditoriaConfiguracionResponse>> ListarAsync();


    }
}
