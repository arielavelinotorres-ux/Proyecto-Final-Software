using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Configuracion
{
    public class AuditoriaConfiguracion
    {
        public int Id { get; set; }
        public int ParametroId { get; set; }
        public int UsuarioId { get; set; }
        public string? ValorAnterior { get; set; }
        public string ValorNuevo { get; set; } = string.Empty;
    }
}
