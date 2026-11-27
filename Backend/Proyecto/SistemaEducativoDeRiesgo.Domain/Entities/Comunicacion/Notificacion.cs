using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
