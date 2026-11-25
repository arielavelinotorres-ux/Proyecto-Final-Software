using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Usuarios
{
    public class Estudiante
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? CodigoEstudiante { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
        public string? ContactoEmergencia { get; set; }
        public DateTime CreadoEn { get; set; }

    }
}
