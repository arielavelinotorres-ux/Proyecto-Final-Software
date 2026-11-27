using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    internal class MensajePrivadoDtos
    {
        public record CrearMensajePrivadoRequest(
            int EmisorId,   
            int ReceptorId,
            string Asunto,
            string Contenido,           
            bool Leido
            );
        public record ActualizarMensajePrivadoRequest(
            int EmisorId,
            int ReceptorId,
            string Asunto,
            string Contenido,
            bool Leido
            );
        public record MensajePrivadoResponse(
            int EmisorId,
            int ReceptorId,
            string Asunto,
            string Contenido,
            bool Leido
            );
    }
}
