using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo
{
    public class EstudianteRiesgo
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public string? EstudianteNombre { get; set; }
        public int CursoId { get; set; }
        public string? CursoTitulo { get; set; }
        public string NivelRiesgo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public DateTime DetectadoEn { get; set; }
        public bool Atendido { get; set; }
    }
}
