using SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Evaluaciones
{
    public interface IRespuestaEstudianteService
    {
        Task<int> CrearAsync(CrearRespuestaEstudianteRequest request);
        Task CalificarAsync(int id, CalificarRespuestaEstudianteRequest request);
        Task EliminarAsync(int id);
        Task<List<RespuestaEstudianteResponse>> ListarPorEstudianteAsync(int estudianteId, int? evaluacionId);
    }
}
