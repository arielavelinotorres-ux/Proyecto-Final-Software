using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Comunicacion
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public NotificacionRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(Notificacion notificacion)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_InsertarNotificacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = notificacion.UsuarioId;
            command.Parameters.Add("@titulo", SqlDbType.NVarChar, 100).Value = notificacion.Titulo;
            command.Parameters.Add("@mensaje", SqlDbType.NVarChar, -1).Value = notificacion.Mensaje;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task MarcarLeidaAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_MarcarNotificacionLeida", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task MarcarTodasLeidasAsync(int usuarioId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_MarcarTodasNotificacionesLeidas", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = usuarioId;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_DesactivarNotificacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Notificacion>> ListarPorUsuarioAsync(int usuarioId)
        {
            var lista = new List<Notificacion>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_ListarNotificaciones", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = usuarioId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Notificacion
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    UsuarioId = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                    Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                    Mensaje = reader.GetString(reader.GetOrdinal("mensaje")),
                    Leido = reader.GetBoolean(reader.GetOrdinal("leido")),
                    FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
                });
            }

            return lista;
        }
    }
}
