using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Academico
{
    internal class MatriculasDtos
    {
        public record CrearMatriculasRequest(
            int EstudianteId,
            int CursoId,
            DateTime FechaMatricula,
            string? Estado);

        public record ActualizarMatriculasRequest(
            int EstudianteId,
            int CursoId,
            DateTime FechaMatricula,
            string? Estado);

        public record MatriculasResponse(
            int EstudianteId,
            int CursoId,
            DateTime FechaMatricula,
            string? Estado);
    }
}
