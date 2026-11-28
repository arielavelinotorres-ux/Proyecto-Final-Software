using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones
{
    public interface IPreguntaService
    {
        Task<int> CrearAsync(CrearPreguntaRequest request);
        Task ActualizarAsync(int id, ActualizarPreguntaRequest request);
        Task EliminarAsync(int id);
        Task<List<PreguntaResponse>> ListarPorEvaluacionAsync(int evaluacionId);
    }
}
