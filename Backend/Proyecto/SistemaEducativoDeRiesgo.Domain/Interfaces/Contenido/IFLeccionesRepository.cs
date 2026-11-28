using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Contenido
{
    public interface IFLeccionesRepository
    {
        Task<int> InsertarAsync(Leccion leccion);
        Task ActualizarAsync(Leccion leccion);
        Task EliminarAsync(int id);
        Task<List<Leccion>> ListarPorSeccionAsync(int seccionId);
    }
}
