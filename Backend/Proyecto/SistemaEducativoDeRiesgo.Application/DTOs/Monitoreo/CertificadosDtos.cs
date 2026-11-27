using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo
{
    internal class CertificadosDtos
    {
        public record CrearCertificadoRequest(
            int EstudianteId,
            int CursoId,
            string CodigoValidacion,
            DateTime FechaEmision,
            string UrlPdf
            );

        public record ActualizarCertificadoRequest(
            int EstudianteId,
            int CursoId,
            string CodigoValidacion,
            DateTime FechaEmision,
            string UrlPdf
            );

        public record CertificadoResponse(
            int EstudianteId,
            int CursoId,
            string CodigoValidacion,
            DateTime FechaEmision,
            string UrlPdf
            );
    }
}
