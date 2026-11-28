using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones
{
    public class Pregunta
    {
        public int Id { get; set; }
        public int EvaluacionId { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public string TipoPregunta { get; set; } = string.Empty;
        public decimal Puntaje { get; set; } = 1.00m;
        public int Orden { get; set; }
    }
}
