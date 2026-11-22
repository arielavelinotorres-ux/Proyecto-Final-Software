using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios
{
    public interface IEstudianteService
    {
        Task<int> CrearAsync(CrearEstudianteRequest request);
        Task ActualizarAsync(int id, ActualizarEstudianteRequest request);
        Task EliminarAsync(int id);
        Task<List<EstudianteResponse>> ListarAsync();
        Task<EstudianteResponse?> ObtenerPorUsuarioIdAsync(int usuarioId);
    }
}
