using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario
{
    public interface IEstudianteRepository
    {
        Task<int> InsertarAsync(Estudiante estudiante);
        Task ActualizarAsync(Estudiante estudiante);
        Task EliminarAsync(int id);
        Task<List<Estudiante>> ListarAsync();
        Task<Estudiante?> ObtenerPorUsuarioIdAsync(int usuarioId);
    }
}
