using SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Comunicacion
{
    public interface IPublicacionForoService
    {
        Task<int> CrearAsync(CrearPublicacionForoRequest request);
        Task ActualizarAsync(int id, ActualizarPublicacionForoRequest request);
        Task EliminarAsync(int id);
        Task<List<PublicacionForoResponse>> ListarPorForoAsync(int foroId);
    }
}
