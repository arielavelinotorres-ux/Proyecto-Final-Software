using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Usuarios
{
    public class DocenteRepository : IDocenteRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public DocenteRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(Docente docente)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_InsertarDocente", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = docente.UsuarioId;
            command.Parameters.Add("@codigo_docente", SqlDbType.NVarChar, 50).Value = (object?)docente.CodigoDocente ?? DBNull.Value;
            command.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = docente.Nombre;
            command.Parameters.Add("@apellido", SqlDbType.NVarChar, 100).Value = docente.Apellido;
            command.Parameters.Add("@especialidad", SqlDbType.NVarChar, 150).Value = (object?)docente.Especialidad ?? DBNull.Value;
            command.Parameters.Add("@telefono", SqlDbType.NVarChar, 20).Value = (object?)docente.Telefono ?? DBNull.Value;
            command.Parameters.Add("@biografia", SqlDbType.NVarChar, -1).Value = (object?)docente.Biografia ?? DBNull.Value;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(Docente docente)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ActualizarDocente", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = docente.Id;
            command.Parameters.Add("@codigo_docente", SqlDbType.NVarChar, 50).Value = (object?)docente.CodigoDocente ?? DBNull.Value;
            command.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = docente.Nombre;
            command.Parameters.Add("@apellido", SqlDbType.NVarChar, 100).Value = docente.Apellido;
            command.Parameters.Add("@especialidad", SqlDbType.NVarChar, 150).Value = (object?)docente.Especialidad ?? DBNull.Value;
            command.Parameters.Add("@telefono", SqlDbType.NVarChar, 20).Value = (object?)docente.Telefono ?? DBNull.Value;
            command.Parameters.Add("@biografia", SqlDbType.NVarChar, -1).Value = (object?)docente.Biografia ?? DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_EliminarDocente", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Docente>> ListarAsync()
        {
            var lista = new List<Docente>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ListarDocentes", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Docente
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    UsuarioId = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                    CodigoDocente = reader.IsDBNull(reader.GetOrdinal("codigo_docente")) ? null : reader.GetString(reader.GetOrdinal("codigo_docente")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                    Especialidad = reader.IsDBNull(reader.GetOrdinal("especialidad")) ? null : reader.GetString(reader.GetOrdinal("especialidad")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                    Biografia = reader.IsDBNull(reader.GetOrdinal("biografia")) ? null : reader.GetString(reader.GetOrdinal("biografia")),
                    CreadoEn = reader.GetDateTime(reader.GetOrdinal("creado_en"))
                });
            }

            return lista;
        }
    }
}
