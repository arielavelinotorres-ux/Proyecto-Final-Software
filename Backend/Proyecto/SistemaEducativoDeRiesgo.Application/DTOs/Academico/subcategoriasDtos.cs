using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Academico
{
    internal class subcategoriasDtos
    {
        public record CrearSubcategoriasRequest(
            int CategoriaId,
            string? Nombre,
            string? Descripcion,
            bool Activo);

        public record ActualizarSubcategoriasRequest(
            int CategoriaId,
            string? Nombre,
            string? Descripcion,
            bool Activo);

        public record SubcategoriasResponse(
            int CategoriaId,
            string? Nombre,
            string? Descripcion,
            bool Activo);
    }
}
