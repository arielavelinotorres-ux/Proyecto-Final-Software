using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.DTOs.Usuarios
{
    public record GenerarTokenRecuperacionRequest(
    int UsuarioId,
    int MinutosExpiracion = 30);

    public record TokenGeneradoResponse(
        int Id,
        string Token,
        DateTime ExpiraEn);

    public record ValidarTokenRequest(string Token);

    public record ValidarTokenResponse(
        int Id,
        int UsuarioId,
        DateTime ExpiraEn,
        bool Usado);

}
