using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Usuarios
{
    public class Usuario
    {

        public int Id { get; set; }
        public int RolId { get; set; }
        public string? RolNombre { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime CreadoEn { get; set; }
        public DateTime? UltimoAcceso { get; set; }

    }
}
