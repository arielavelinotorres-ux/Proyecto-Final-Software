using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo
{
    public class ProgresoLeccion
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int LeccionId { get; set; }
        public string? LeccionTitulo { get; set; }
        public int CursoId { get; set; }
        public bool Completado { get; set; }
        public DateTime? FechaCompletado { get; set; }
    }
}
