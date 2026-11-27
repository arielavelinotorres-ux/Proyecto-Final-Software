using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    public record CrearMensajePrivadoRequest(
        int EmisorId,
        int ReceptorId,
        string? Asunto,
        string Contenido);

    public record MensajeRecibidoResponse(
        int Id,
        int EmisorId,
        string EmisorNombre,
        int ReceptorId,
        string? Asunto,
        string Contenido,
        bool Leido,
        DateTime FechaEnvio);

    public record MensajeEnviadoResponse(
        int Id,
        int EmisorId,
        int ReceptorId,
        string ReceptorNombre,
        string? Asunto,
        string Contenido,
        bool Leido,
        DateTime FechaEnvio);
}
