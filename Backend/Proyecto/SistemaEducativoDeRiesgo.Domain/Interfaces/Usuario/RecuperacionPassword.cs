using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario
{
    public interface IRecuperacionPasswordRepository
    {
        Task<int> InsertarTokenAsync(int usuarioId, string tokenHash, DateTime expiraEn);
        Task<RecuperacionPassword?> ValidarTokenAsync(string tokenHash);
        Task MarcarTokenUsadoAsync(int id);
    }
}
