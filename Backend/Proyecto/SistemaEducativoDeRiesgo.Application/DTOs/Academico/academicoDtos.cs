using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Academico
{
    internal class academicoDtos
    {
        public record CrearacademicoRequest(
            int AcademicoId,
            string? nombre,
            string?descripcion,
            bool activo);


        public record ActualizaracademicoRequest(
            string? nombre,
            string?descripcion,
            bool activo);


        public record academicoResponse(
            int AcademicoId,
            string? nombre,
            string? descripcion,
            bool activo);

    }
}
