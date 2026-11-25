using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Usuarios
{
    public class RecuperacionPassword
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiraEn { get; set; }
        public bool Usado { get; set; }

    }
}
