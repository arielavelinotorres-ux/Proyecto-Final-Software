using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones
{
    public interface IOpcionPreguntaService
    {
        Task<int> CrearAsync(CrearOpcionPreguntaRequest request);
        Task ActualizarAsync(int id, ActualizarOpcionPreguntaRequest request);
        Task EliminarAsync(int id);
        Task<List<OpcionPreguntaResponse>> ListarPorPreguntaAsync(int preguntaId);
    }
}
