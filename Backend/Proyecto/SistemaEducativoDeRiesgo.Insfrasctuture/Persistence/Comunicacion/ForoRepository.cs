using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Comunicacion
{
    public class ForoRepository : IForoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ForoRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(Foro foro)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_InsertarForo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = foro.CursoId;
            command.Parameters.Add("@titulo", SqlDbType.NVarChar, 150).Value = foro.Titulo;
            command.Parameters.Add("@descripcion", SqlDbType.NVarChar, -1).Value = (object?)foro.Descripcion ?? DBNull.Value;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(Foro foro)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_ActualizarForo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = foro.Id;
            command.Parameters.Add("@titulo", SqlDbType.NVarChar, 150).Value = foro.Titulo;
            command.Parameters.Add("@descripcion", SqlDbType.NVarChar, -1).Value = (object?)foro.Descripcion ?? DBNull.Value;
            command.Parameters.Add("@estado", SqlDbType.NVarChar, 20).Value = foro.Estado;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_DesactivarForo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Foro>> ListarPorCursoAsync(int cursoId)
        {
            var lista = new List<Foro>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_ListarForos", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = cursoId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Foro
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    CursoId = reader.GetInt32(reader.GetOrdinal("curso_id")),
                    Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                    Estado = reader.GetString(reader.GetOrdinal("estado")),
                    CreadoEn = reader.GetDateTime(reader.GetOrdinal("creado_en"))
                });
            }

            return lista;
        }
    }
}
