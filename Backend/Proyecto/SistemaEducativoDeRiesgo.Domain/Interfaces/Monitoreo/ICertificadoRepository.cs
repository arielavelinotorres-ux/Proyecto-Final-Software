using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo
{
    public interface ICertificadoRepository
    {
        Task<int> InsertarAsync(Certificado certificado);
        Task ActualizarAsync(Certificado certificado);
        Task<Certificado?> ValidarPorCodigoAsync(string codigoValidacion);
        Task EliminarAsync(int id);
        Task<List<Certificado>> ListarPorEstudianteAsync(int estudianteId);
    }
}
