 using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Contenido
{
    internal class SeccionesDtos
    {
        public record CrearSeccionRequest(
            int CursoId,
            string Titulo,
            int Orden
            );
        public record ActualizarSeccionRequest(
            int CursoId,
            string Titulo,
            int Orden
            );

        public record SeccionResponse(
            int CursoId,
            string Titulo,
            int Orden
            );
    }
}
