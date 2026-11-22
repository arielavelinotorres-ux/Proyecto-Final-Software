using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Usuarios
{
    public record CrearEstudianteRequest(
    int UsuarioId,
    string? CodigoEstudiante,
    string Nombre,
    string Apellido,
    DateOnly FechaNacimiento,
    string? Telefono,
    string? ContactoEmergencia);

    public record ActualizarEstudianteRequest(
        string? CodigoEstudiante,
        string Nombre,
        string Apellido,
        DateOnly FechaNacimiento,
        string? Telefono,
        string? ContactoEmergencia);

    public record EstudianteResponse(
        int Id,
        int UsuarioId,
        string? CodigoEstudiante,
        string Nombre,
        string Apellido,
        DateOnly FechaNacimiento,
        string? Telefono,
        string? ContactoEmergencia,
        DateTime CreadoEn);

}
