using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Usuarios
{
    public class Docente
    {

        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? CodigoDocente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Especialidad { get; set; }
        public string? Telefono { get; set; }
        public string? Biografia { get; set; }
        public DateTime CreadoEn { get; set; }

    }
}
