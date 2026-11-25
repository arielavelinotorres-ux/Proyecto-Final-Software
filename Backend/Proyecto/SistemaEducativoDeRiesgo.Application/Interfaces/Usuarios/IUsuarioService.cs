using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;

namespace SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios
{
    public interface IUsuarioService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<int> CrearAsync(CrearUsuarioRequest request);
        Task ActualizarPerfilAsync(int id, ActualizarUsuarioPerfilRequest request);
        Task ActualizarPasswordAsync(int id, ActualizarPasswordRequest request);
        Task DesactivarAsync(int id);
        Task ActivarAsync(int id);
        Task<List<UsuarioResponse>> ListarAsync();
        Task CerrarTodasSesionesAsync(int usuarioId);

    }
}
