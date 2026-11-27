using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Comunicacion
{
    public record CrearForoRequest(
        int CursoId,
        string Titulo,
        string? Descripcion);

    public record ActualizarForoRequest(
        string Titulo,
        string? Descripcion,
        string Estado);

    public record ForoResponse(
        int Id,
        int CursoId,
        string Titulo,
        string? Descripcion,
        string Estado,
        DateTime CreadoEn);
}

