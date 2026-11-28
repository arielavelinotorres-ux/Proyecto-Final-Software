using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones
{
    public class Evaluacion
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal PuntajeMaximo { get; set; } = 100.00m;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaLimite { get; set; }
        public string Estado { get; set; } = "Activo";
        public DateTime CreadoEn { get; set; }
    }
}
