using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Evaluaciones
{
    public record CrearOpcionPreguntaRequest(
        int PreguntaId,
        string TextoOpcion,
        bool EsCorrecta,
        int Orden);

    public record ActualizarOpcionPreguntaRequest(
        string TextoOpcion,
        bool EsCorrecta,
        int Orden);

    public record OpcionPreguntaResponse(
        int Id,
        int PreguntaId,
        string TextoOpcion,
        bool EsCorrecta,
        int Orden);
}
