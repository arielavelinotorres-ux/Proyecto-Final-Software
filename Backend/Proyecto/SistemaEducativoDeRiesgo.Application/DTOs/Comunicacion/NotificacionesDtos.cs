using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    public record CrearNotificacionRequest(
         int UsuarioId,
         string Titulo,
         string Mensaje);

    public record NotificacionResponse(
        int Id,
        int UsuarioId,
        string Titulo,
        string Mensaje,
        bool Leido,
        DateTime FechaCreacion);
}
