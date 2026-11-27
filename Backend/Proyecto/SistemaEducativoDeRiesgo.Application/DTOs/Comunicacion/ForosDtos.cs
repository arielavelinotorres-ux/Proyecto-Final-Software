using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    internal class ForosDtos
    {
        public record CrearForoRequest(
            int ForoId,
            string? Titulo,
            string? Descripcion,
            string? Estado
            );

        public record ActualizarForoRequest(
            int ForoId,
            string? Titulo,
            string? Descripcion,
            string? Estado
            );

        public record ForoResponse(
            int ForoId,
            string? Titulo,
            string? Descripcion,
            string? Estado
            );
    }
}
