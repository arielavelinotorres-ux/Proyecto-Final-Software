using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario
{
    public interface IDocenteRepository
    {
        Task<int> InsertarAsync(Docente docente);
        Task ActualizarAsync(Docente docente);
        Task EliminarAsync(int id);
        Task<List<Docente>> ListarAsync();

    }
}
