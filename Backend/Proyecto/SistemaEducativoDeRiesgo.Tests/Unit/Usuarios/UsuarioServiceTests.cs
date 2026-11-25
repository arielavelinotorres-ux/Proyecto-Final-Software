using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.Services.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;
using SistemaEducativoDeRiesgo.Insfrasctuture.Security;

namespace SistemaEducativoDeRiesgo.Tests.Unit.Usuarios
{
    public class UsuarioServiceTests
    {
        private readonly PasswordHasher _hasher = new();

        [Fact]
        public async Task LoginAsync_ConCredencialesValidas_DebeRetornarLoginResponse()
        {
            var repo = new FakeUsuarioRepository();
            var hash = _hasher.Hash("Admin123");
            repo.UsuarioLogin = new Usuario
            {
                Id = 1,
                RolId = 2,
                RolNombre = "Docente",
                Correo = "profe@lms.com",
                PasswordHash = hash,
                Activo = true
            };

            var service = new UsuarioService(repo, _hasher);

            var resultado = await service.LoginAsync(new LoginRequest("profe@lms.com", "Admin123"));

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Docente", resultado.RolNombre);
        }

        [Fact]
        public async Task LoginAsync_ConUsuarioInactivo_DebeRetornarNull()
        {
            var repo = new FakeUsuarioRepository();
            repo.UsuarioLogin = new Usuario
            {
                Id = 1,
                RolId = 3,
                Correo = "inactivo@lms.com",
                PasswordHash = _hasher.Hash("Admin123"),
                Activo = false
            };

            var service = new UsuarioService(repo, _hasher);

            var resultado = await service.LoginAsync(new LoginRequest("inactivo@lms.com", "Admin123"));

            Assert.Null(resultado);
        }

        [Fact]
        public async Task LoginAsync_ConPasswordIncorrecto_DebeRetornarNull()
        {
            var repo = new FakeUsuarioRepository();
            repo.UsuarioLogin = new Usuario
            {
                Id = 1,
                RolId = 3,
                Correo = "alumno@lms.com",
                PasswordHash = _hasher.Hash("ClaveReal123"),
                Activo = true
            };

            var service = new UsuarioService(repo, _hasher);

            var resultado = await service.LoginAsync(new LoginRequest("alumno@lms.com", "ClaveMala"));

            Assert.Null(resultado);
        }

        [Fact]
        public async Task CrearAsync_ConPasswordCorto_DebeLanzarArgumentException()
        {
            var service = new UsuarioService(new FakeUsuarioRepository(), _hasher);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CrearAsync(new CrearUsuarioRequest(3, "nuevo@lms.com", "123")));
        }

        [Fact]
        public async Task CrearAsync_ConDatosValidos_DebeHashearPasswordYRetornarId()
        {
            var repo = new FakeUsuarioRepository();
            var service = new UsuarioService(repo, _hasher);

            var id = await service.CrearAsync(new CrearUsuarioRequest(3, "  nuevo@lms.com  ", "Secreta123"));

            Assert.Equal(10, id);
            Assert.NotNull(repo.UltimoUsuarioInsertado);
            Assert.Equal("nuevo@lms.com", repo.UltimoUsuarioInsertado.Correo);
            Assert.True(_hasher.Verify("Secreta123", repo.UltimoUsuarioInsertado.PasswordHash));
        }

        private class FakeUsuarioRepository : IUsuarioRepository
        {
            public Usuario? UsuarioLogin { get; set; }
            public Usuario? UltimoUsuarioInsertado { get; private set; }

            public Task<Usuario?> ObtenerParaLoginAsync(string correo) => Task.FromResult(UsuarioLogin);

            public Task<int> InsertarAsync(Usuario usuario)
            {
                UltimoUsuarioInsertado = usuario;
                return Task.FromResult(10);
            }

            public Task ActualizarPerfilAsync(int id, int rolId, string correo) => Task.CompletedTask;
            public Task ActualizarPasswordAsync(int id, string nuevoPasswordHash) => Task.CompletedTask;
            public Task DesactivarAsync(int id) => Task.CompletedTask;
            public Task ActivarAsync(int id) => Task.CompletedTask;
            public Task<List<Usuario>> ListarAsync() => Task.FromResult(new List<Usuario>());
            public Task CerrarTodasSesionesAsync(int usuarioId) => Task.CompletedTask;
        }
    }
}
