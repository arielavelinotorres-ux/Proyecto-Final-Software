using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Services.Usuarios
{
    public class RecuperacionPasswordService : IRecuperacionPasswordService
    {
        private readonly IRecuperacionPasswordRepository _repository;

        public RecuperacionPasswordService(IRecuperacionPasswordRepository repository)
        {
            _repository = repository;
        }

        public async Task<TokenGeneradoResponse> GenerarTokenAsync(GenerarTokenRecuperacionRequest request)
        {
            var tokenPlano = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var tokenHash = CalcularSha256(tokenPlano);
            var expiraEn = DateTime.Now.AddMinutes(request.MinutosExpiracion <= 0 ? 30 : request.MinutosExpiracion);

            var id = await _repository.InsertarTokenAsync(request.UsuarioId, tokenHash, expiraEn);
            return new TokenGeneradoResponse(id, tokenPlano, expiraEn);
        }

        public async Task<ValidarTokenResponse?> ValidarTokenAsync(string tokenPlano)
        {
            if (string.IsNullOrWhiteSpace(tokenPlano))
                return null;

            var tokenHash = CalcularSha256(tokenPlano.Trim());
            var registro = await _repository.ValidarTokenAsync(tokenHash);

            return registro is null
                ? null
                : new ValidarTokenResponse(registro.Id, registro.UsuarioId, registro.ExpiraEn, registro.Usado);
        }

        public Task MarcarTokenUsadoAsync(int id) => _repository.MarcarTokenUsadoAsync(id);

        private static string CalcularSha256(string texto)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(texto));
            return Convert.ToHexString(bytes);
        }
    }
}
