using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    internal class NotificacionesDtos
    {
        public record CrearNotificacionRequest(
            int UsuarioId,
            string? Titulo,
            string? Mensaje,
            bool Leido
            );
        public record ActualizarNotificacionRequest(
            int UsuarioId,
            string? Titulo,
            string? Mensaje,
            bool Leido
            );
        public record NotificacionResponse(
            int UsuarioId,
            string? Titulo,
            string? Mensaje,
            bool Leido
            );
    }
}
