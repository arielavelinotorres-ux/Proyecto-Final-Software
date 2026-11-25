using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Usuarios
{
    public record LoginRequest(string Correo, string Password);

    public record LoginResponse(
        int Id,
        int RolId,
        string? RolNombre,
        string Correo,
        bool Activo);

    public record CrearUsuarioRequest(
        int RolId,
        string Correo,
        string Password);

    public record ActualizarUsuarioPerfilRequest(
        int RolId,
        string Correo);

    public record ActualizarPasswordRequest(string NuevoPassword);

    public record UsuarioResponse(
        int Id,
        int RolId,
        string? RolNombre,
        string Correo,
        bool Activo,
        DateTime CreadoEn,
        DateTime? UltimoAcceso);
}
