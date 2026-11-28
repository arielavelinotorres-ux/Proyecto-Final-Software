using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Evaluaciones;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Evaluaciones;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Evaluaciones
{
    public class EntregaTareaRepository : IEntregaTareaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public EntregaTareaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(EntregaTarea entrega)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_InsertarEntregaTarea", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@evaluacion_id", SqlDbType.Int).Value = entrega.EvaluacionId;
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = entrega.EstudianteId;
            command.Parameters.Add("@archivo_url", SqlDbType.NVarChar, 255).Value = (object?)entrega.ArchivoUrl ?? DBNull.Value;
            command.Parameters.Add("@contenido_texto", SqlDbType.NVarChar, -1).Value = (object?)entrega.ContenidoTexto ?? DBNull.Value;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarCalificacionAsync(int id, decimal calificacion, string? retroalimentacion, string estado)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ActualizarEntregaTarea", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            var pCalificacion = command.Parameters.Add("@calificacion", SqlDbType.Decimal);
            pCalificacion.Precision = 5;
            pCalificacion.Scale = 2;
            pCalificacion.Value = calificacion;

            command.Parameters.Add("@retroalimentacion", SqlDbType.NVarChar, -1).Value = (object?)retroalimentacion ?? DBNull.Value;
            command.Parameters.Add("@estado", SqlDbType.NVarChar, 20).Value = estado;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_EliminarEntregaTarea", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<EntregaTarea>> ListarPorEvaluacionAsync(int evaluacionId)
        {
            var lista = new List<EntregaTarea>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Evaluaciones.SP_ListarEntregasPorEvaluacion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@evaluacion_id", SqlDbType.Int).Value = evaluacionId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new EntregaTarea
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EvaluacionId = reader.GetInt32(reader.GetOrdinal("evaluacion_id")),
                    EstudianteId = reader.GetInt32(reader.GetOrdinal("estudiante_id")),
                    EstudianteNombre = reader.GetString(reader.GetOrdinal("estudiante_nombre")),
                    ArchivoUrl = reader.IsDBNull(reader.GetOrdinal("archivo_url")) ? null : reader.GetString(reader.GetOrdinal("archivo_url")),
                    ContenidoTexto = reader.IsDBNull(reader.GetOrdinal("contenido_texto")) ? null : reader.GetString(reader.GetOrdinal("contenido_texto")),
                    FechaEntrega = reader.GetDateTime(reader.GetOrdinal("fecha_entrega")),
                    Calificacion = reader.IsDBNull(reader.GetOrdinal("calificacion")) ? null : reader.GetDecimal(reader.GetOrdinal("calificacion")),
                    Retroalimentacion = reader.IsDBNull(reader.GetOrdinal("retroalimentacion")) ? null : reader.GetString(reader.GetOrdinal("retroalimentacion")),
                    Estado = reader.GetString(reader.GetOrdinal("estado"))
                });
            }

            return lista;
        }
    }
}
