using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios
{
    public interface IDocenteService
    {
        Task<int> CrearAsync(CrearDocenteRequest request);
        Task ActualizarAsync(int id, ActualizarDocenteRequest request);
        Task EliminarAsync(int id);
        Task<List<DocenteResponse>> ListarAsync();

    }
}
