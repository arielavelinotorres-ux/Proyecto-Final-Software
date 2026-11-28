using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Contenido
{
    public interface IFRecursosRepository
    {
        Task<int> InsertarAsync(Recurso recurso);
        Task ActualizarAsync(Recurso recurso);
        Task EliminarAsync(int id);
        Task<List<Recurso>> ListarPorLeccionAsync(int leccionId);
    }
}
