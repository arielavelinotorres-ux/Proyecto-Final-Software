using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones
{
    public interface IRespuestaEstudianteRepository
    {
        Task<int> InsertarAsync(RespuestaEstudiante respuesta);
        Task CalificarAsync(int id, bool esCorrecta, decimal puntajeObtenido);
        Task EliminarAsync(int id);
        Task<List<RespuestaEstudiante>> ListarPorEstudianteAsync(int estudianteId, int? evaluacionId);
    }

}
