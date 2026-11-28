using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo
{
    public interface IProgresoLeccionRepository
    {
        Task GuardarAsync(int estudianteId, int leccionId, bool completado);
        Task EliminarAsync(int id);
        Task<List<ProgresoLeccion>> ListarPorEstudianteAsync(int estudianteId, int? cursoId);
    }
}
