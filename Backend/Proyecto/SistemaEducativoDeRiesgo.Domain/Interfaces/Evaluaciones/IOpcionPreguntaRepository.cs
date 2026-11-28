using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones
{
    public interface IOpcionPreguntaRepository
    {
        Task<int> InsertarAsync(OpcionPregunta opcion);
        Task ActualizarAsync(OpcionPregunta opcion);
        Task EliminarAsync(int id);
        Task<List<OpcionPregunta>> ListarPorPreguntaAsync(int preguntaId);
    }

}
