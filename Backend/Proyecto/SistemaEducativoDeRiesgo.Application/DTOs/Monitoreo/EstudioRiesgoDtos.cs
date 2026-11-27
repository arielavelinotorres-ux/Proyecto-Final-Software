using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo
{
    internal class EstudioRiesgoDtos
    {
        public record CrearEstudioRiesgoRequest(
            int EstudianteId,
            string Nombre,
            string Descripcion,
            DateTime FechaCreacion
            );

        public record ActualizarEstudioRiesgoRequest(
            int EstudianteId,
            string Nombre,
            string Descripcion,
            DateTime FechaCreacion
            );

         public record EstudioRiesgoResponse(
            int EstudianteId,
            string Nombre,
            string Descripcion,
            DateTime FechaCreacion
            );
    }
}
