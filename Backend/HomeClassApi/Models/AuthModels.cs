using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeClassApi.Models
{
    public class LoginRequest
    {
        [Required]
        public string Correo { get; set; }

        [Required]
        public string Clave { get; set; }
    }

    public class CambioClaveRequest
    {
        [Required]
        public string Actual { get; set; }

        [Required]
        public string Nueva { get; set; }
    }

    public class RestablecerClaveRequest
    {
        [Required]
        public string Nueva { get; set; }
    }

    public class UsuarioRespuesta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
        public bool SolicitudRestablecimiento { get; set; }
        public string TipoAdmin { get; set; }
        public List<string> PermisosAdmin { get; set; }
    }

    public class UsuarioSeed
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string ClaveHash { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public int IntentosFallidos { get; set; }
        public bool Bloqueado { get; set; }
        public bool SolicitudRestablecimiento { get; set; }
        public string TipoAdmin { get; set; }
        public string PermisosAdminCsv { get; set; }
    }

    public class UsuarioRequest
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public string TipoAdmin { get; set; }
        public List<string> PermisosAdmin { get; set; }
        public string Clave { get; set; }
        public bool Activo { get; set; }
    }

    public class CursoRequest
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Periodo { get; set; }
        public string Descripcion { get; set; }
        public int? DocenteId { get; set; }
    }

    public class MatriculaRequest
    {
        public int EstudianteId { get; set; }
    }

    public class PerfilRequest
    {
        public string Nombre { get; set; }
    }

    public class CursoSeed
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Periodo { get; set; }
        public string Descripcion { get; set; }
        public int? DocenteId { get; set; }
    }

    public class RespuestaError
    {
        public string Mensaje { get; set; }
    }

    public class EvaluacionRequest
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public decimal Peso { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Instrucciones { get; set; }
        public bool RequiereEntrega { get; set; }
        public string Extensiones { get; set; }
        public int MaxMb { get; set; }
        public bool PermiteTardia { get; set; }
    }

    public class NotaRequest
    {
        public decimal? Nota { get; set; }
    }

    public class AsistenciaRequest
    {
        public string Fecha { get; set; }
        public List<AsistenciaRegistroRequest> Registros { get; set; }
    }

    public class AsistenciaRegistroRequest
    {
        public int EstudianteId { get; set; }
        public bool Presente { get; set; }
    }

    public class AvisoRequest
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
    }
}
