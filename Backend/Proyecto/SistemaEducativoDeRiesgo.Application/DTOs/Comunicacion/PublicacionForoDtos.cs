using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    public record CrearPublicacionForoRequest(
         int ForoId,
         int UsuarioId,
         int? PublicacionPadreId,
         string Contenido);

    public record ActualizarPublicacionForoRequest(
        string Contenido);

    public record PublicacionForoResponse(
        int Id,
        int ForoId,
        int UsuarioId,
        string AutorRol,
        string AutorNombre,
        int? PublicacionPadreId,
        string Contenido,
        DateTime CreadoEn);
}
