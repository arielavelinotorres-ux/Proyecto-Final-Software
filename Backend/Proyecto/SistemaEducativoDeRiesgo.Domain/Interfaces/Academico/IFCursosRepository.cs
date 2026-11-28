using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Academico
{
    public interface IFCursosRepository
    {
        Task<int> InsertarAsync(Curso curso);
        Task ActualizarAsync(Curso curso);
        Task EliminarAsync(int id);
        Task<List<Curso>> ListarAsync();
        Task<Curso> ObtenerPorIdAsync(int id);
    }
}
