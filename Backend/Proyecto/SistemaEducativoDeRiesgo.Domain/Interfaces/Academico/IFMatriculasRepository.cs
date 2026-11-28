using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Academico
{
    public interface IFMatriculasRepository
    {
        Task<int> InsertarAsync(Matricula matricula);
        Task ActualizarAsync(Matricula matricula);
        Task EliminarAsync(int id);
        Task<List<Matricula>> ListarAsync();
        Task<Matricula> ObtenerPorIdAsync(int id);
    }
}
