using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Contenido
{
    public interface IRecursosService
    {
        Task<int> InsertarAsync(Recurso recurso);
        Task ActualizarAsync(Recurso recurso);
        Task EliminarAsync(int id);
        Task<List<Recurso>> ListarPorLeccionAsync(int leccionId);
    }
}
