using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Contenido
{
    public interface IFSeccionesRepository
    {
        Task<int> InsertarAsync(Seccion seccion);
        Task ActualizarAsync(Seccion seccion);
        Task EliminarAsync(int id);
        Task<List<Seccion>> ListarPorCursoAsync(int cursoId);
    }
}
