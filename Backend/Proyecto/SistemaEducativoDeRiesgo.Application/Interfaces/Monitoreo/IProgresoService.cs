using SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Monitoreo
{
    public interface IProgresoLeccionService
    {
        Task GuardarAsync(GuardarProgresoLeccionRequest request);
        Task EliminarAsync(int id);
        Task<List<ProgresoLeccionResponse>> ListarPorEstudianteAsync(int estudianteId, int? cursoId);
    }
}
