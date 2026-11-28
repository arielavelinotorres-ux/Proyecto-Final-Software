using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones
{
    public record CrearPreguntaRequest(
        int EvaluacionId,
        string Enunciado,
        string TipoPregunta,
        decimal Puntaje,
        int Orden);

    public record ActualizarPreguntaRequest(
        string Enunciado,
        string TipoPregunta,
        decimal Puntaje,
        int Orden);

    public record PreguntaResponse(
        int Id,
        int EvaluacionId,
        string Enunciado,
        string TipoPregunta,
        decimal Puntaje,
        int Orden);
}
