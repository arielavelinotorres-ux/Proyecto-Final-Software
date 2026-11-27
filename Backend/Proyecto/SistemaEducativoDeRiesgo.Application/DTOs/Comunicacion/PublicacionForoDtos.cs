using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    internal class PublicacionForoDtos
    {
        public record CrearPublicacionForoRequest(
            int ForoId,
            int UsuarioId,
            int PublicacionId,
            string? Contenido   
            );
        public record ActualizarPublicacionForoRequest(
             int ForoId,
            int UsuarioId,
            int PublicacionId,
            string? Contenido
            );
        public record PublicacionForoResponse(
             int ForoId,
            int UsuarioId,
            int PublicacionId,
            string? Contenido
            );
    }
}
