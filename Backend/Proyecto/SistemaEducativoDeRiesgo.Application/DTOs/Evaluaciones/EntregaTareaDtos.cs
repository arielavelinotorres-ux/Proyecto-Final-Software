using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones
{
    internal class EntregaTareaDtos
    {
        public record CrearEntregaTareaRequest(
            int EvaluacionId,
            int EstudianteId,
            string? ArchivoUrl,
            string? ContenidoTexto,
            decimal? Calificacion,
            string? Retroalimentacion,
            string? Estado
            );
     
        public record ActualizarEntregaTareaRequest(
            int EvaluacionId,
            int EstudianteId,
            string? ArchivoUrl,
            string? ContenidoTexto,
            decimal? Calificacion,
            string? Retroalimentacion,
            string? Estado
            );

        public record EntregaTareaResponse(
            int EvaluacionId,
            int EstudianteId,
            string? ArchivoUrl,
            string? ContenidoTexto,
            decimal? Calificacion,
            string? Retroalimentacion,
            string? Estado
            );
    }
}

