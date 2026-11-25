using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Usuarios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public UsuarioRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Usuario?> ObtenerParaLoginAsync(string correo)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ObtenerUsuarioParaLogin", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@correo", SqlDbType.NVarChar, 150).Value = correo;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                RolId = reader.GetInt32(reader.GetOrdinal("rol_id")),
                RolNombre = TryGetString(reader, "rol_nombre"),
                Correo = reader.GetString(reader.GetOrdinal("correo")),
                PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                Activo = reader.GetBoolean(reader.GetOrdinal("activo"))
            };
        }

        public async Task<int> InsertarAsync(Usuario usuario)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_InsertarUsuario", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@rol_id", SqlDbType.Int).Value = usuario.RolId;
            command.Parameters.Add("@correo", SqlDbType.NVarChar, 150).Value = usuario.Correo;
            command.Parameters.Add("@password_hash", SqlDbType.NVarChar, 255).Value = usuario.PasswordHash;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarPerfilAsync(int id, int rolId, string correo)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ActualizarUsuarioPerfil", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@rol_id", SqlDbType.Int).Value = rolId;
            command.Parameters.Add("@correo", SqlDbType.NVarChar, 150).Value = correo;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task ActualizarPasswordAsync(int id, string nuevoPasswordHash)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ActualizarUsuarioPassword", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@password_hash", SqlDbType.NVarChar, 255).Value = nuevoPasswordHash;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_DesactivarUsuario", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task ActivarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ActivarUsuario", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Usuario>> ListarAsync()
        {
            var lista = new List<Usuario>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ListarUsuarios", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    RolId = reader.GetInt32(reader.GetOrdinal("rol_id")),
                    RolNombre = TryGetString(reader, "rol_nombre"),
                    Correo = reader.GetString(reader.GetOrdinal("correo")),
                    Activo = reader.GetBoolean(reader.GetOrdinal("activo")),
                    CreadoEn = reader.GetDateTime(reader.GetOrdinal("creado_en")),
                    UltimoAcceso = TryGetNullableDateTime(reader, "ultimo_acceso")
                });
            }

            return lista;
        }

        public async Task CerrarTodasSesionesAsync(int usuarioId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_CerrarTodasSesiones", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = usuarioId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private static string? TryGetString(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return reader.IsDBNull(i) ? null : reader.GetString(i);
            }
            return null;
        }

        private static DateTime? TryGetNullableDateTime(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return reader.IsDBNull(i) ? null : reader.GetDateTime(i);
            }
            return null;
        }
    }
}
