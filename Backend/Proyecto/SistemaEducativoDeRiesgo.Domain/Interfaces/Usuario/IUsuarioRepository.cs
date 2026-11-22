using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario
{
    public interface IUsuarioRepository
    {

        Task<Entities.Usuarios.Usuario?> ObtenerParaLoginAsync(string correo);
        Task<int> InsertarAsync(Entities.Usuarios.Usuario usuario);
        Task ActualizarPerfilAsync(int id, int rolId, string correo);
        Task ActualizarPasswordAsync(int id, string nuevoPasswordHash);
        Task DesactivarAsync(int id);
        Task ActivarAsync(int id);
        Task<List<Entities.Usuarios.Usuario>> ListarAsync();
        Task CerrarTodasSesionesAsync(int usuarioId);

    }
}
