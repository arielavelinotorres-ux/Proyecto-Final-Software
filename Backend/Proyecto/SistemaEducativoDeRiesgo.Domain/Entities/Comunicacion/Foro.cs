using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion
{
    public class Foro
    {

        public int Id { get; set; }
        public int CursoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = "Abierto";
        public DateTime CreadoEn { get; set; }

    }
}
