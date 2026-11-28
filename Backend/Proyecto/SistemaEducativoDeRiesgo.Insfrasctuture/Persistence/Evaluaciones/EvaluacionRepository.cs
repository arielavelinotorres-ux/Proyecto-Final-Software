using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Evaluaciones
{
    public class EvaluacionRepository : IEvaluacionRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public EvaluacionRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(Evaluacion evaluacion)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_InsertarEvaluacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = evaluacion.CursoId;
            command.Parameters.Add("@titulo", SqlDbType.NVarChar, 150).Value = evaluacion.Titulo;
            command.Parameters.Add("@descripcion", SqlDbType.NVarChar, -1).Value = (object?)evaluacion.Descripcion ?? DBNull.Value;
            command.Parameters.Add("@tipo", SqlDbType.NVarChar, 30).Value = evaluacion.Tipo;

            var pPuntaje = command.Parameters.Add("@puntaje_maximo", SqlDbType.Decimal);
            pPuntaje.Precision = 5;
            pPuntaje.Scale = 2;
            pPuntaje.Value = evaluacion.PuntajeMaximo;

            command.Parameters.Add("@fecha_inicio", SqlDbType.DateTime2).Value = evaluacion.FechaInicio == default ? DBNull.Value : evaluacion.FechaInicio;
            command.Parameters.Add("@fecha_limite", SqlDbType.DateTime2).Value = (object?)evaluacion.FechaLimite ?? DBNull.Value;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(Evaluacion evaluacion)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ActualizarEvaluacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = evaluacion.Id;
            command.Parameters.Add("@titulo", SqlDbType.NVarChar, 150).Value = evaluacion.Titulo;
            command.Parameters.Add("@descripcion", SqlDbType.NVarChar, -1).Value = (object?)evaluacion.Descripcion ?? DBNull.Value;
            command.Parameters.Add("@tipo", SqlDbType.NVarChar, 30).Value = evaluacion.Tipo;

            var pPuntaje = command.Parameters.Add("@puntaje_maximo", SqlDbType.Decimal);
            pPuntaje.Precision = 5;
            pPuntaje.Scale = 2;
            pPuntaje.Value = evaluacion.PuntajeMaximo;

            command.Parameters.Add("@fecha_inicio", SqlDbType.DateTime2).Value = evaluacion.FechaInicio == default ? DBNull.Value : evaluacion.FechaInicio;
            command.Parameters.Add("@fecha_limite", SqlDbType.DateTime2).Value = (object?)evaluacion.FechaLimite ?? DBNull.Value;
            command.Parameters.Add("@estado", SqlDbType.NVarChar, 20).Value = evaluacion.Estado;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_DesactivarEvaluacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Evaluacion>> ListarPorCursoAsync(int cursoId)
        {
            var lista = new List<Evaluacion>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ListarEvaluacionesPorCurso", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = cursoId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Evaluacion
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    CursoId = reader.GetInt32(reader.GetOrdinal("curso_id")),
                    Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                    Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                    PuntajeMaximo = reader.GetDecimal(reader.GetOrdinal("puntaje_maximo")),
                    FechaInicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                    FechaLimite = reader.IsDBNull(reader.GetOrdinal("fecha_limite")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha_limite")),
                    Estado = reader.GetString(reader.GetOrdinal("estado")),
                    CreadoEn = reader.GetDateTime(reader.GetOrdinal("creado_en"))
                });
            }

            return lista;
        }
    }
}
