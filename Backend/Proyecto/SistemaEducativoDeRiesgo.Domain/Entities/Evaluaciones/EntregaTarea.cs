using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones
{
    public class EntregaTarea
    {
        public int Id { get; set; }
        public int EvaluacionId { get; set; }
        public int EstudianteId { get; set; }
        public string? EstudianteNombre { get; set; }
        public string? ArchivoUrl { get; set; }
        public string? ContenidoTexto { get; set; }
        public DateTime FechaEntrega { get; set; }
        public decimal? Calificacion { get; set; }
        public string? Retroalimentacion { get; set; }
        public string Estado { get; set; } = "Enviado";
    }
}
