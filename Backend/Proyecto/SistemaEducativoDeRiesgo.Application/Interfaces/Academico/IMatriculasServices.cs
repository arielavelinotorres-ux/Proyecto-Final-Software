using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Academico
{
    public interface IMatriculasServices
    {
        Task<int> InsertarAsync(Matricula matricula);
        Task ActualizarAsync(Matricula matricula);
        Task EliminarAsync(int id);
        Task<List<Matricula>> ListarAsync();
        Task<Matricula> ObtenerPorIdAsync(int id);
    }
}
