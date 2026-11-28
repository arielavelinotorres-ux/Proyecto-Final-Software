using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Configuracion
{
    public interface IFParametrosSistema
    {
        Task<int> CrearAsync(CrearParametrosSistemaRequest request);
        Task ActualizarAsync(int id, ActualizarParametrosSistemaRequest request);
        Task EliminarAsync(int id);
        Task<List<ParametrosSistemaResponse>> ListarAsync();

    }
}
