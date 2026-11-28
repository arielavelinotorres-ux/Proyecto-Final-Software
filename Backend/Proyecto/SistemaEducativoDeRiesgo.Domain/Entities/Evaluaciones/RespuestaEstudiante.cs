using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones
{
    public class RespuestaEstudiante
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int PreguntaId { get; set; }
        public int? OpcionSeleccionadaId { get; set; }
        public string? RespuestaTexto { get; set; }
        public bool? EsCorrecta { get; set; }
        public decimal? PuntajeObtenido { get; set; }
        public DateTime FechaRespuesta { get; set; }
    }
}
