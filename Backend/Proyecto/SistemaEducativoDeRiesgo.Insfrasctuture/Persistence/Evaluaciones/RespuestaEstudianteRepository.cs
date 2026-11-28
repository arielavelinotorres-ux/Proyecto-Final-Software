using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Evaluaciones
{
    public class RespuestaEstudianteRepository : IRespuestaEstudianteRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public RespuestaEstudianteRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(RespuestaEstudiante respuesta)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_InsertarRespuestaEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = respuesta.EstudianteId;
            command.Parameters.Add("@pregunta_id", SqlDbType.Int).Value = respuesta.PreguntaId;
            command.Parameters.Add("@opcion_seleccionada_id", SqlDbType.Int).Value = (object?)respuesta.OpcionSeleccionadaId ?? DBNull.Value;
            command.Parameters.Add("@respuesta_texto", SqlDbType.NVarChar, -1).Value = (object?)respuesta.RespuestaTexto ?? DBNull.Value;
            command.Parameters.Add("@es_correcta", SqlDbType.Bit).Value = (object?)respuesta.EsCorrecta ?? DBNull.Value;

            var pPuntaje = command.Parameters.Add("@puntaje_obtenido", SqlDbType.Decimal);
            pPuntaje.Precision = 5;
            pPuntaje.Scale = 2;
            pPuntaje.Value = (object?)respuesta.PuntajeObtenido ?? DBNull.Value;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task CalificarAsync(int id, bool esCorrecta, decimal puntajeObtenido)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ActualizarRespuestaEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@es_correcta", SqlDbType.Bit).Value = esCorrecta;

            var pPuntaje = command.Parameters.Add("@puntaje_obtenido", SqlDbType.Decimal);
            pPuntaje.Precision = 5;
            pPuntaje.Scale = 2;
            pPuntaje.Value = puntajeObtenido;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_EliminarRespuestaEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<RespuestaEstudiante>> ListarPorEstudianteAsync(int estudianteId, int? evaluacionId)
        {
            var lista = new List<RespuestaEstudiante>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ListarRespuestasEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = estudianteId;
            command.Parameters.Add("@evaluacion_id", SqlDbType.Int).Value = (object?)evaluacionId ?? DBNull.Value;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new RespuestaEstudiante
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EstudianteId = reader.GetInt32(reader.GetOrdinal("estudiante_id")),
                    PreguntaId = reader.GetInt32(reader.GetOrdinal("pregunta_id")),
                    OpcionSeleccionadaId = reader.IsDBNull(reader.GetOrdinal("opcion_seleccionada_id")) ? null : reader.GetInt32(reader.GetOrdinal("opcion_seleccionada_id")),
                    RespuestaTexto = reader.IsDBNull(reader.GetOrdinal("respuesta_texto")) ? null : reader.GetString(reader.GetOrdinal("respuesta_texto")),
                    EsCorrecta = reader.IsDBNull(reader.GetOrdinal("es_correcta")) ? null : reader.GetBoolean(reader.GetOrdinal("es_correcta")),
                    PuntajeObtenido = reader.IsDBNull(reader.GetOrdinal("puntaje_obtenido")) ? null : reader.GetDecimal(reader.GetOrdinal("puntaje_obtenido")),
                    FechaRespuesta = reader.GetDateTime(reader.GetOrdinal("fecha_respuesta"))
                });
            }

            return lista;
        }
    }
}
