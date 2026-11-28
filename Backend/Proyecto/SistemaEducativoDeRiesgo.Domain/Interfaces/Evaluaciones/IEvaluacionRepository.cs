using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones
{
    public interface IEvaluacionRepository
    {
        Task<int> InsertarAsync(Evaluacion evaluacion);
        Task ActualizarAsync(Evaluacion evaluacion);
        Task DesactivarAsync(int id);
        Task<List<Evaluacion>> ListarPorCursoAsync(int cursoId);
    }
}
