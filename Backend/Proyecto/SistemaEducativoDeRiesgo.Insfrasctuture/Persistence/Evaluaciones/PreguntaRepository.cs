using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Evaluaciones
{
    public class PreguntaRepository : IPreguntaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public PreguntaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(Pregunta pregunta)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_InsertarPregunta", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@evaluacion_id", SqlDbType.Int).Value = pregunta.EvaluacionId;
            command.Parameters.Add("@enunciado", SqlDbType.NVarChar, -1).Value = pregunta.Enunciado;
            command.Parameters.Add("@tipo_pregunta", SqlDbType.NVarChar, 30).Value = pregunta.TipoPregunta;

            var pPuntaje = command.Parameters.Add("@puntaje", SqlDbType.Decimal);
            pPuntaje.Precision = 5;
            pPuntaje.Scale = 2;
            pPuntaje.Value = pregunta.Puntaje;

            command.Parameters.Add("@orden", SqlDbType.Int).Value = pregunta.Orden;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(Pregunta pregunta)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ActualizarPregunta", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = pregunta.Id;
            command.Parameters.Add("@enunciado", SqlDbType.NVarChar, -1).Value = pregunta.Enunciado;
            command.Parameters.Add("@tipo_pregunta", SqlDbType.NVarChar, 30).Value = pregunta.TipoPregunta;

            var pPuntaje = command.Parameters.Add("@puntaje", SqlDbType.Decimal);
            pPuntaje.Precision = 5;
            pPuntaje.Scale = 2;
            pPuntaje.Value = pregunta.Puntaje;

            command.Parameters.Add("@orden", SqlDbType.Int).Value = pregunta.Orden;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_EliminarPregunta", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Pregunta>> ListarPorEvaluacionAsync(int evaluacionId)
        {
            var lista = new List<Pregunta>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ListarPreguntasPorEvaluacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@evaluacion_id", SqlDbType.Int).Value = evaluacionId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Pregunta
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EvaluacionId = reader.GetInt32(reader.GetOrdinal("evaluacion_id")),
                    Enunciado = reader.GetString(reader.GetOrdinal("enunciado")),
                    TipoPregunta = reader.GetString(reader.GetOrdinal("tipo_pregunta")),
                    Puntaje = reader.GetDecimal(reader.GetOrdinal("puntaje")),
                    Orden = reader.GetInt32(reader.GetOrdinal("orden"))
                });
            }

            return lista;
        }
    }
}
