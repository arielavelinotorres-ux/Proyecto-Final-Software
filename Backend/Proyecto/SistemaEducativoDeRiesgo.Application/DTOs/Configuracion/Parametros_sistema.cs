using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Configuracion
{
    internal class Parametros_sistema
    {
        public record CrearParametrosSistemaRequest(
            string Clave,
            string? Valor,
            string? Descripcion,
            string? TipoDato
            );

        public record ActualizarParametrosSistemaRequest(
            string Clave,
            string? Valor,
            string? Descripcion,
            string? TipoDato
            );
        public record ParametrosSistemaResponse(
            string Clave,
            string? Valor,
            string? Descripcion,
            string? TipoDato
            );
    }
}
