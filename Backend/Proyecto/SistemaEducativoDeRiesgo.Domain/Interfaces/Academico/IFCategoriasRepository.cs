using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Academico
{
    public interface IFCategoriasRepository
    {
        Task<int> InsertarAsync(Categoria categoria);
        Task ActualizarAsync(Categoria categoria);
        Task EliminarAsync(int id);
        Task<List<Categoria>> ListarAsync();
        Task<Categoria> ObtenerPorIdAsync(int id);

    }
}
