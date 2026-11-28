using SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Monitoreo
{
    public interface ICertificadoService
    {
        Task<int> CrearAsync(CrearCertificadoRequest request);
        Task ActualizarAsync(int id, ActualizarCertificadoRequest request);
        Task<ValidarCertificadoResponse?> ValidarPorCodigoAsync(string codigoValidacion);
        Task EliminarAsync(int id);
        Task<List<CertificadoResponse>> ListarPorEstudianteAsync(int estudianteId);
    }
}
