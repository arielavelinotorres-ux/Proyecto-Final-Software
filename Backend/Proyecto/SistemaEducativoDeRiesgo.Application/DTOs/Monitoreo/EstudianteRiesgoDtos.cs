using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo
{
    public record CrearEstudianteRiesgoRequest(
        int EstudianteId,
        int CursoId,
        string NivelRiesgo,
        string Motivo);

    public record ActualizarEstudianteRiesgoRequest(
        string NivelRiesgo,
        string Motivo,
        bool Atendido);

    public record EstudianteRiesgoResponse(
        int Id,
        int EstudianteId,
        string EstudianteNombre,
        int CursoId,
        string CursoTitulo,
        string NivelRiesgo,
        string Motivo,
        DateTime DetectadoEn,
        bool Atendido);
}
