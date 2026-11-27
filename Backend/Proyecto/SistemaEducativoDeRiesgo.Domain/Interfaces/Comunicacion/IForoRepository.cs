using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion
{
    public interface IForoRepository
    {
            Task<int> InsertarAsync(Foro foro);
            Task ActualizarAsync(Foro foro);
            Task DesactivarAsync(int id);
            Task<List<Foro>> ListarPorCursoAsync(int cursoId);
        

    }
}
