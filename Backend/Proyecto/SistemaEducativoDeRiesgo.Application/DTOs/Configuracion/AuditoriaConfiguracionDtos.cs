using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Configuracion
{
    internal class AuditoriaConfiguracionDtos
    {
        public record CrearAuditoriaConfiguracionRequest(
            int ParametrosId,
            int UsuarioId,
            string? ValorAnterior,
            string? ValorNuevo
            );
        public record ActualizarAuditoriaConfiguracionRequest(
            int ParametrosId,
            int UsuarioId,
            string? ValorAnterior,
            string? ValorNuevo
            );

        public record AuditoriaConfiguracionResponse(
            int ParametrosId,
            int UsuarioId,
            string? ValorAnterior,
            string? ValorNuevo
            );
    }
}
