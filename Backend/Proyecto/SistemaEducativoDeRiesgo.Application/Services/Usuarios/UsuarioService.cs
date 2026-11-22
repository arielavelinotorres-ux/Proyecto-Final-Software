using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioService(IUsuarioRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            var usuario = await _repository.ObtenerParaLoginAsync(request.Correo.Trim());
            if (usuario is null || !usuario.Activo)
                return null;

            if (!_passwordHasher.Verify(request.Password, usuario.PasswordHash))
                return null;

            return new LoginResponse(usuario.Id, usuario.RolId, usuario.RolNombre, usuario.Correo, usuario.Activo);
        }

        public async Task<int> CrearAsync(CrearUsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");

            var usuario = new Usuario
            {
                RolId = request.RolId,
                Correo = request.Correo.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password)
            };

            return await _repository.InsertarAsync(usuario);
        }

        public Task ActualizarPerfilAsync(int id, ActualizarUsuarioPerfilRequest request)
            => _repository.ActualizarPerfilAsync(id, request.RolId, request.Correo.Trim());

        public Task ActualizarPasswordAsync(int id, ActualizarPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NuevoPassword) || request.NuevoPassword.Length < 6)
                throw new ArgumentException("La nueva contraseña debe tener al menos 6 caracteres.");

            var hash = _passwordHasher.Hash(request.NuevoPassword);
            return _repository.ActualizarPasswordAsync(id, hash);
        }

        public Task DesactivarAsync(int id) => _repository.DesactivarAsync(id);

        public Task ActivarAsync(int id) => _repository.ActivarAsync(id);

        public async Task<List<UsuarioResponse>> ListarAsync()
        {
            var usuarios = await _repository.ListarAsync();
            return usuarios.Select(u => new UsuarioResponse(
                u.Id, u.RolId, u.RolNombre, u.Correo, u.Activo, u.CreadoEn, u.UltimoAcceso
            )).ToList();
        }

        public Task CerrarTodasSesionesAsync(int usuarioId)
            => _repository.CerrarTodasSesionesAsync(usuarioId);
    }
}
