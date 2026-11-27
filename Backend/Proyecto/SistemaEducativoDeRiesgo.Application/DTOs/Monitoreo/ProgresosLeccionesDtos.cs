using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo
{
    internal class ProgresosLeccionesDtos
    {
        public record CrearProgresoLeccionRequest(
            int EstudianteId,
            int LeccionId,
            bool Completado,
            DateTime FechaProgreso
            );

        public record ActualizarProgresoLeccionRequest(
            int EstudianteId,
            int LeccionId,
            bool Completado,
            DateTime FechaProgreso
            );

        public record ProgresoLeccionResponse(
            int EstudianteId,
            int LeccionId,
            bool Completado,
            DateTime FechaProgreso
            );
    }
}
