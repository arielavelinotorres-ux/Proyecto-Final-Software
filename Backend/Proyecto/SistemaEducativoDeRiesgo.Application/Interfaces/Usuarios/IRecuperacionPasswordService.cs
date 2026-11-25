using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios
{
    public interface IRecuperacionPasswordService
    {
        Task<TokenGeneradoResponse> GenerarTokenAsync(GenerarTokenRecuperacionRequest request);
        Task<ValidarTokenResponse?> ValidarTokenAsync(string tokenPlano);
        Task MarcarTokenUsadoAsync(int id);
    }
}
