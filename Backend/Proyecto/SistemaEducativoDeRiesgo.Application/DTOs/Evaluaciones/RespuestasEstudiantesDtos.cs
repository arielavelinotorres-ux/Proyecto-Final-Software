using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones
{

    public record CrearRespuestaEstudianteRequest(
        int EstudianteId,
        int PreguntaId,
        int? OpcionSeleccionadaId,
        string? RespuestaTexto,
        bool? EsCorrecta,
        decimal? PuntajeObtenido);

    public record CalificarRespuestaEstudianteRequest(
        bool EsCorrecta,
        decimal PuntajeObtenido);

    public record RespuestaEstudianteResponse(
        int Id,
        int EstudianteId,
        int PreguntaId,
        int? OpcionSeleccionadaId,
        string? RespuestaTexto,
        bool? EsCorrecta,
        decimal? PuntajeObtenido,
        DateTime FechaRespuesta);
}
