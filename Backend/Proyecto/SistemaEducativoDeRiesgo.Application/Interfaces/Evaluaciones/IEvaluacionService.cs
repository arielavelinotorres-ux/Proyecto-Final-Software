using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones
{
    public interface IEvaluacionService
    {
        Task<int> CrearAsync(CrearEvaluacionRequest request);
        Task ActualizarAsync(int id, ActualizarEvaluacionRequest request);
        Task DesactivarAsync(int id);
        Task<List<EvaluacionResponse>> ListarPorCursoAsync(int cursoId);
    }
}
