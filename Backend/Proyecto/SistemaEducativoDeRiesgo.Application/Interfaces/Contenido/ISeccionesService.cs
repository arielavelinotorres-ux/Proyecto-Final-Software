using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Contenido
{
    public interface ISeccionesService
    {
        Task<int> InsertarAsync(Seccion seccion);
        Task ActualizarAsync(Seccion seccion);
        Task EliminarAsync(int id);
        Task<List<Seccion>> ListarPorCursoAsync(int cursoId);
    }
}
