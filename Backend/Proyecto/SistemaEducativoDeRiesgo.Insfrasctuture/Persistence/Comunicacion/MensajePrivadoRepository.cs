using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Comunicacion
{
    public class MensajePrivadoRepository : IMensajePrivadoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public MensajePrivadoRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(MensajePrivado mensaje)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_InsertarMensajePrivado", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@emisor_id", SqlDbType.Int).Value = mensaje.EmisorId;
            command.Parameters.Add("@receptor_id", SqlDbType.Int).Value = mensaje.ReceptorId;
            command.Parameters.Add("@asunto", SqlDbType.NVarChar, 150).Value = (object?)mensaje.Asunto ?? DBNull.Value;
            command.Parameters.Add("@contenido", SqlDbType.NVarChar, -1).Value = mensaje.Contenido;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task MarcarComoLeidoAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_MarcarMensajeComoLeido", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_DesactivarMensajePrivado", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<MensajePrivado>> ListarRecibidosAsync(int receptorId)
        {
            var lista = new List<MensajePrivado>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_ListarMensajesPrivados", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@receptor_id", SqlDbType.Int).Value = receptorId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new MensajePrivado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EmisorId = reader.GetInt32(reader.GetOrdinal("emisor_id")),
                    EmisorNombre = reader.GetString(reader.GetOrdinal("emisor_nombre")),
                    ReceptorId = reader.GetInt32(reader.GetOrdinal("receptor_id")),
                    Asunto = reader.IsDBNull(reader.GetOrdinal("asunto")) ? null : reader.GetString(reader.GetOrdinal("asunto")),
                    Contenido = reader.GetString(reader.GetOrdinal("contenido")),
                    Leido = reader.GetBoolean(reader.GetOrdinal("leido")),
                    FechaEnvio = reader.GetDateTime(reader.GetOrdinal("fecha_envio"))
                });
            }

            return lista;
        }

        public async Task<List<MensajePrivado>> ListarEnviadosAsync(int emisorId)
        {
            var lista = new List<MensajePrivado>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_ListarMensajesEnviados", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@emisor_id", SqlDbType.Int).Value = emisorId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new MensajePrivado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EmisorId = reader.GetInt32(reader.GetOrdinal("emisor_id")),
                    ReceptorId = reader.GetInt32(reader.GetOrdinal("receptor_id")),
                    ReceptorNombre = reader.GetString(reader.GetOrdinal("receptor_nombre")),
                    Asunto = reader.IsDBNull(reader.GetOrdinal("asunto")) ? null : reader.GetString(reader.GetOrdinal("asunto")),
                    Contenido = reader.GetString(reader.GetOrdinal("contenido")),
                    Leido = reader.GetBoolean(reader.GetOrdinal("leido")),
                    FechaEnvio = reader.GetDateTime(reader.GetOrdinal("fecha_envio"))
                });
            }

            return lista;
        }
    }
}
