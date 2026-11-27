using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion
{
    public interface IForoService
    {
        Task<int> CrearAsync(CrearForoRequest request);
        Task ActualizarAsync(int id, ActualizarForoRequest request);
        Task DesactivarAsync(int id);
        Task<List<ForoResponse>> ListarPorCursoAsync(int cursoId);
    }
}
