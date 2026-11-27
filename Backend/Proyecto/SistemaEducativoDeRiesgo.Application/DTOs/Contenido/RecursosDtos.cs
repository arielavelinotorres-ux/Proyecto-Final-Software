using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Contenido
{
    internal class RecursosDtos
    {
        public record CrearRecursoRequest(
            int LeccionId,
            string Nombre,
            string TipoArchivo,
            string UrlRecurso
            );

        public record ActualizarRecursoRequest(
            int LeccionId,
            string Nombre,
            string TipoArchivo,
            string UrlRecurso
            );

        public record RecursoResponse(
            int LeccionId,
            string Nombre,
            string TipoArchivo,
            string UrlRecurso
            );
    }
}
