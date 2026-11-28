using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Configuracion
{
    public interface IFParametros_SistmasRepository
    {
        Task<List<Parametros_Sistema>> ListarAsync();
            Task<Parametros_Sistema> ObtenerPorIdAsync(int id);
            Task<int> InsertarAsync(Parametros_Sistema config);
            Task ActualizarAsync(Parametros_Sistema config);
            Task EliminarAsync(int id);

    }
}

