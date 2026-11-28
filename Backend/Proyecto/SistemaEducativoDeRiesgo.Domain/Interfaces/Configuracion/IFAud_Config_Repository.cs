using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Configuracion
{
    public interface IFAud_Config_Repository
    {  
        Task <List<FAud_Config>> ListarAsync();
        Task <FAud_Config> ObtenerPorIdAsync(int id);
        Task <int> InsertarAsync(FAud_Config config);
        Task ActualizarAsync(FAud_Config config);
        Task EliminarAsync(int id);

    }
}
