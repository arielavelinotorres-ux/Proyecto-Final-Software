using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones
{
    public interface IEntregaTareaService
    {
        Task<int> CrearAsync(CrearEntregaTareaRequest request);
        Task ActualizarAsync(int id, ActualizarEntregaTareaRequest request);
        Task EliminarAsync(int id);
        Task<List<EntregaTareaResponse>> ListarPorEvaluacionAsync(int evaluacionId);
    }
}
