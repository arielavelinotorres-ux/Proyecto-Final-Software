using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Usuarios
{
    public record CrearDocenteRequest(
    int UsuarioId,
    string? CodigoDocente,
    string Nombre,
    string Apellido,
    string? Especialidad,
    string? Telefono,
    string? Biografia);

    public record ActualizarDocenteRequest(
        string? CodigoDocente,
        string Nombre,
        string Apellido,
        string? Especialidad,
        string? Telefono,
        string? Biografia);

    public record DocenteResponse(
        int Id,
        int UsuarioId,
        string? CodigoDocente,
        string Nombre,
        string Apellido,
        string? Especialidad,
        string? Telefono,
        string? Biografia,
        DateTime CreadoEn);
}
