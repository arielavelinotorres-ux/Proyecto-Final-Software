using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones
{
    public class OpcionPregunta
    {
        public int Id { get; set; }
        public int PreguntaId { get; set; }
        public string TextoOpcion { get; set; } = string.Empty;
        public bool EsCorrecta { get; set; }
        public int Orden { get; set; }
    }
}
