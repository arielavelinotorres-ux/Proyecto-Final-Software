using SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Monitoreo
{
    public interface IEstudianteRiesgoService
    {
        Task<int> CrearAsync(CrearEstudianteRiesgoRequest request);
        Task ActualizarAsync(int id, ActualizarEstudianteRiesgoRequest request);
        Task EliminarAsync(int id);
        Task<List<EstudianteRiesgoResponse>> ListarAsync(int? cursoId);
    }
}
