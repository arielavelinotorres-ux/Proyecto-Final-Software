using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Configuracion
{
    public class ParametrosSistema
    {
        public int Id { get; set; }

        public string Clave { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string TipoDato { get; set; } = string.Empty;
    }
}
