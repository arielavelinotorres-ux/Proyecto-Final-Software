using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo
{
    public class Certificado
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public string? EstudianteNombre { get; set; }
        public int CursoId { get; set; }
        public string? CursoTitulo { get; set; }
        public string CodigoValidacion { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public string? UrlPdf { get; set; }
    }
}
