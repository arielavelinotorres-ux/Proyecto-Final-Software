using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.Services.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Tests.Unit.Usuarios
{
    public class RecuperacionPasswordServiceTests
    {
        [Fact]
        public async Task GenerarYValidarToken_DebeGuardarHashYValidarConTokenPlano()
        {
            var repo = new FakeRecuperacionRepository();
            var service = new RecuperacionPasswordService(repo);

            var generado = await service.GenerarTokenAsync(new GenerarTokenRecuperacionRequest(5, 30));

            Assert.NotEmpty(generado.Token);
            Assert.NotEqual(generado.Token, repo.TokenHashGuardado);

            var validado = await service.ValidarTokenAsync(generado.Token);

            Assert.NotNull(validado);
            Assert.Equal(5, validado.UsuarioId);
            Assert.False(validado.Usado);
        }

        [Fact]
        public async Task ValidarTokenAsync_ConTokenVacio_DebeRetornarNull()
        {
            var service = new RecuperacionPasswordService(new FakeRecuperacionRepository());

            var resultado = await service.ValidarTokenAsync("   ");

            Assert.Null(resultado);
        }

        private class FakeRecuperacionRepository : IRecuperacionPasswordRepository
        {
            public string? TokenHashGuardado { get; private set; }
            private int _usuarioId;
            private DateTime _expiraEn;

            public Task<int> InsertarTokenAsync(int usuarioId, string tokenHash, DateTime expiraEn)
            {
                _usuarioId = usuarioId;
                TokenHashGuardado = tokenHash;
                _expiraEn = expiraEn;
                return Task.FromResult(1);
            }

            public Task<RecuperacionPassword?> ValidarTokenAsync(string tokenHash)
            {
                if (tokenHash != TokenHashGuardado)
                    return Task.FromResult<RecuperacionPassword?>(null);

                return Task.FromResult<RecuperacionPassword?>(new RecuperacionPassword
                {
                    Id = 1,
                    UsuarioId = _usuarioId,
                    TokenHash = tokenHash,
                    ExpiraEn = _expiraEn,
                    Usado = false
                });
            }

            public Task MarcarTokenUsadoAsync(int id) => Task.CompletedTask;
        }
    }
}
