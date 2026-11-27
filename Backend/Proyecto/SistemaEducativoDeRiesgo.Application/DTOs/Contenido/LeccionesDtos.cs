using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Contenido
{
    internal class LeccionesDtos
    {
        public record CrearLeccionRequest(
            int SeccionId,
            string? Titulo,
            string? ContenidoTexto,
            string? ContenidoVideo,
            int Duracion,
            int orden,
            string? EstGratuito
            );

        public record ActualizarLeccionRequest(
            int SeccionId,
            string? Titulo,
            string? ContenidoTexto,
            string? ContenidoVideo,
            int Duracion,
            int orden,
            string? EstGratuito
            );
        public record LeccionResponse(
            int SeccionId,
            string? Titulo,
            string? ContenidoTexto,
            string? ContenidoVideo,
            int Duracion,
            int orden,
            string? EstGratuito
            );
            
    }
}
