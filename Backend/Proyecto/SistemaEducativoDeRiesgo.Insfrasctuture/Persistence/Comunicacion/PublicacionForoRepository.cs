using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Comunicacion
{
    public class PublicacionForoRepository : IPublicacionForoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public PublicacionForoRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(PublicacionForo publicacion)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_InsertarPublicacionForo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@foro_id", SqlDbType.Int).Value = publicacion.ForoId;
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = publicacion.UsuarioId;
            command.Parameters.Add("@publicacion_padre_id", SqlDbType.Int).Value = (object?)publicacion.PublicacionPadreId ?? DBNull.Value;
            command.Parameters.Add("@contenido", SqlDbType.NVarChar, -1).Value = publicacion.Contenido;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(int id, string contenido)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_ActualizarPublicacionForo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@contenido", SqlDbType.NVarChar, -1).Value = contenido;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_DesactivarPublicacionForo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<PublicacionForo>> ListarPorForoAsync(int foroId)
        {
            var lista = new List<PublicacionForo>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Comunicacion.SP_ListarPublicacionesForo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@foro_id", SqlDbType.Int).Value = foroId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new PublicacionForo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    ForoId = reader.GetInt32(reader.GetOrdinal("foro_id")),
                    UsuarioId = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                    AutorRol = reader.GetString(reader.GetOrdinal("autor_rol")),
                    AutorNombre = reader.GetString(reader.GetOrdinal("autor_nombre")),
                    PublicacionPadreId = reader.IsDBNull(reader.GetOrdinal("publicacion_padre_id")) ? null : reader.GetInt32(reader.GetOrdinal("publicacion_padre_id")),
                    Contenido = reader.GetString(reader.GetOrdinal("contenido")),
                    CreadoEn = reader.GetDateTime(reader.GetOrdinal("creado_en"))
                });
            }

            return lista;
        }
    }
}
