using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Contenido
{
    public interface ILeccionesServices
    {
        Task<int> InsertarAsync(Leccion leccion);
        Task ActualizarAsync(Leccion leccion);
        Task EliminarAsync(int id);
        Task<List<Leccion>> ListarPorSeccionAsync(int seccionId);
    }
}
