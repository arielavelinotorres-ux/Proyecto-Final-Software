using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion
{
    public class MensajePrivado
    {

        public int Id { get; set; }
        public int EmisorId { get; set; }
        public string? EmisorNombre { get; set; }
        public int ReceptorId { get; set; }
        public string? ReceptorNombre { get; set; }
        public string? Asunto { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public DateTime FechaEnvio { get; set; }

    }
}
