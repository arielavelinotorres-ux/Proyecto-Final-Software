using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Usuarios
{
    public class RecuperacionPasswordRepository : IRecuperacionPasswordRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public RecuperacionPasswordRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarTokenAsync(int usuarioId, string tokenHash, DateTime expiraEn)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_InsertarTokenRecuperacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = usuarioId;
            command.Parameters.Add("@token_hash", SqlDbType.NVarChar, 255).Value = tokenHash;
            command.Parameters.Add("@expira_en", SqlDbType.DateTime2).Value = expiraEn;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<RecuperacionPassword?> ValidarTokenAsync(string tokenHash)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ValidarTokenRecuperacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@token_hash", SqlDbType.NVarChar, 255).Value = tokenHash;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new RecuperacionPassword
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                UsuarioId = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                TokenHash = tokenHash,
                ExpiraEn = reader.GetDateTime(reader.GetOrdinal("expira_en")),
                Usado = reader.GetBoolean(reader.GetOrdinal("usado"))
            };
        }

        public async Task MarcarTokenUsadoAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_MarcarTokenUsado", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
