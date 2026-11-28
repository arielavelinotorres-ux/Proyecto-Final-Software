using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Academico
{
    public class Matriculas
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public int EstudianteId { get; set; }
        public DateTime FechaMatricula { get; set; }
    }
}
