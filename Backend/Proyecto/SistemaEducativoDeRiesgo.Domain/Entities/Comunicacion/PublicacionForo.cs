using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion
{
    public class PublicacionForo
    {
        public int Id { get; set; }
        public int ForoId { get; set; }
        public int UsuarioId { get; set; }
        public string? AutorRol { get; set; }
        public string? AutorNombre { get; set; }
        public int? PublicacionPadreId { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }

    }
}
