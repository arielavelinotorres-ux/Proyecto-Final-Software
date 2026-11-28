using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Academico
{
    public interface IFsubcategoriaRepository
    {
        Task<int> InsertarAsync(Subcategoria subcategoria);
        Task ActualizarAsync(Subcategoria subcategoria);
        Task EliminarAsync(int id);
        Task<List<Subcategoria>> ListarAsync();
        Task<Subcategoria> ObtenerPorIdAsync(int id);
    }
}
