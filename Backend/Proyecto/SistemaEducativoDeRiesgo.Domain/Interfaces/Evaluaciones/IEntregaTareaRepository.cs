using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones
{
    public interface IEntregaTareaRepository
    {
        Task<int> InsertarAsync(EntregaTarea entrega);
        Task ActualizarCalificacionAsync(int id, decimal calificacion, string? retroalimentacion, string estado);
        Task EliminarAsync(int id);
        Task<List<EntregaTarea>> ListarPorEvaluacionAsync(int evaluacionId);
    }
}
