using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Evaluaciones
{
    public class OpcionPreguntaRepository : IOpcionPreguntaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public OpcionPreguntaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(OpcionPregunta opcion)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_InsertarOpcionPregunta", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@pregunta_id", SqlDbType.Int).Value = opcion.PreguntaId;
            command.Parameters.Add("@texto_opcion", SqlDbType.NVarChar, -1).Value = opcion.TextoOpcion;
            command.Parameters.Add("@es_correcta", SqlDbType.Bit).Value = opcion.EsCorrecta;
            command.Parameters.Add("@orden", SqlDbType.Int).Value = opcion.Orden;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(OpcionPregunta opcion)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ActualizarOpcionPregunta", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = opcion.Id;
            command.Parameters.Add("@texto_opcion", SqlDbType.NVarChar, -1).Value = opcion.TextoOpcion;
            command.Parameters.Add("@es_correcta", SqlDbType.Bit).Value = opcion.EsCorrecta;
            command.Parameters.Add("@orden", SqlDbType.Int).Value = opcion.Orden;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_EliminarOpcionPregunta", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<OpcionPregunta>> ListarPorPreguntaAsync(int preguntaId)
        {
            var lista = new List<OpcionPregunta>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ListarOpcionesPorPregunta", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@pregunta_id", SqlDbType.Int).Value = preguntaId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new OpcionPregunta
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    PreguntaId = reader.GetInt32(reader.GetOrdinal("pregunta_id")),
                    TextoOpcion = reader.GetString(reader.GetOrdinal("texto_opcion")),
                    EsCorrecta = reader.GetBoolean(reader.GetOrdinal("es_correcta")),
                    Orden = reader.GetInt32(reader.GetOrdinal("orden"))
                });
            }

            return lista;
        }
    }
}
