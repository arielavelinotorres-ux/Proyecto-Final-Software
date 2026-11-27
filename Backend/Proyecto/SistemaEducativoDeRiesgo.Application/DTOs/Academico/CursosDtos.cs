using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Academico
{
    internal class CursosDtos
    {
        public record CrearCursosRequest(
            int SubcategoriaId,
            int DocenteId,
            string? Titulo,
            string? Descripcion,
            string? CodigoCurso,
            string? ImagenPortada,
            string? Estado);



        internal class ActyalizarCursosRequest(
            int SubcategoriaId,
            int DocenteId,
            string? Titulo,
            string? Descripcion,
            string? CodigoCurso,
            string? ImagenPortada,
            string? Estado);

        internal class CursosResponse(
            int SubcategoriaId,
            int DocenteId,
            string? Titulo,
            string? Descripcion,
            string? CodigoCurso,
            string? ImagenPortada,
            string? Estado);

    }

    
}
