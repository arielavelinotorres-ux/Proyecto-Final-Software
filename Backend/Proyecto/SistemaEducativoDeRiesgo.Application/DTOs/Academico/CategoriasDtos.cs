using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Academico
{
    internal class CategoriasDtos
    {
        public record CrearCategoriasRequest(
            int CategoriasId,
            string? nombre,
            string?descripcion,
            bool activo);


        public record ActualizarCategoriasRequest(
            int CategoriasId,
            string? nombre,
            string?descripcion,
            bool activo);


        public record CategoriasResponse(
            int CategoriasId,
            int AcademicoId,
            string? nombre,
            string? descripcion,
            bool activo);

    }
}
