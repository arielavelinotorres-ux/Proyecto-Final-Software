using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo
{
    public record GuardarProgresoLeccionRequest(
         int EstudianteId,
         int LeccionId,
         bool Completado);

    public record ProgresoLeccionResponse(
        int Id,
        int EstudianteId,
        int LeccionId,
        string LeccionTitulo,
        int CursoId,
        bool Completado,
        DateTime? FechaCompletado);
}
