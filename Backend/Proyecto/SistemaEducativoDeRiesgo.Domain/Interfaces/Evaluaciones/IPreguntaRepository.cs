using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones
{
    public interface IPreguntaRepository
    {
        Task<int> InsertarAsync(Pregunta pregunta);
        Task ActualizarAsync(Pregunta pregunta);
        Task EliminarAsync(int id);
        Task<List<Pregunta>> ListarPorEvaluacionAsync(int evaluacionId);
    }
}
