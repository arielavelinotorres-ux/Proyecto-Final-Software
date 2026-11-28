using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones
{
    public record CrearEvaluacionRequest(
        int CursoId,
        string Titulo,
        string? Descripcion,
        string Tipo,
        decimal PuntajeMaximo,
        DateTime? FechaInicio,
        DateTime? FechaLimite);

    public record ActualizarEvaluacionRequest(
        string Titulo,
        string? Descripcion,
        string Tipo,
        decimal PuntajeMaximo,
        DateTime? FechaInicio,
        DateTime? FechaLimite,
        string Estado);

    public record EvaluacionResponse(
        int Id,
        int CursoId,
        string Titulo,
        string? Descripcion,
        string Tipo,
        decimal PuntajeMaximo,
        DateTime FechaInicio,
        DateTime? FechaLimite,
        string Estado,
        DateTime CreadoEn);
}
