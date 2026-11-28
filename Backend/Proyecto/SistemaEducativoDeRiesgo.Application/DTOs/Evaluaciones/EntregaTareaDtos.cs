using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones
{
    public record CrearEntregaTareaRequest(
        int EvaluacionId,
        int EstudianteId,
        string? ArchivoUrl,
        string? ContenidoTexto);

    public record ActualizarEntregaTareaRequest(
        decimal Calificacion,
        string? Retroalimentacion,
        string Estado);

    public record EntregaTareaResponse(
        int Id,
        int EvaluacionId,
        int EstudianteId,
        string EstudianteNombre,
        string? ArchivoUrl,
        string? ContenidoTexto,
        DateTime FechaEntrega,
        decimal? Calificacion,
        string? Retroalimentacion,
        string Estado);
}

