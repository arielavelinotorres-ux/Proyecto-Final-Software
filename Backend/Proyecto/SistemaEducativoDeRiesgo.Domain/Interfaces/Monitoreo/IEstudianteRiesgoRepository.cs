using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo
{
    public interface IEstudianteRiesgoRepository
    {
        Task<int> InsertarAsync(EstudianteRiesgo registro);
        Task ActualizarAsync(EstudianteRiesgo registro);
        Task EliminarAsync(int id);
        Task<List<EstudianteRiesgo>> ListarAsync(int? cursoId);
    }
}
