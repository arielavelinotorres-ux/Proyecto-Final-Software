using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo
{
    public record CrearCertificadoRequest(
        int EstudianteId,
        int CursoId,
        string CodigoValidacion,
        string? UrlPdf);

    public record ActualizarCertificadoRequest(
        string CodigoValidacion,
        string? UrlPdf);

    public record CertificadoResponse(
        int Id,
        int EstudianteId,
        int CursoId,
        string CursoTitulo,
        string CodigoValidacion,
        DateTime FechaEmision,
        string? UrlPdf);

    public record ValidarCertificadoResponse(
        int Id,
        string CodigoValidacion,
        DateTime FechaEmision,
        string? UrlPdf,
        string EstudianteNombre,
        string CursoTitulo);
}
