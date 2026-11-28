using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Monitoreo
{
    public class ProgresoLeccionRepository : IProgresoLeccionRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ProgresoLeccionRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task GuardarAsync(int estudianteId, int leccionId, bool completado)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_GuardarProgresoLeccion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = estudianteId;
            command.Parameters.Add("@leccion_id", SqlDbType.Int).Value = leccionId;
            command.Parameters.Add("@completado", SqlDbType.Bit).Value = completado;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_EliminarProgresoLeccion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<ProgresoLeccion>> ListarPorEstudianteAsync(int estudianteId, int? cursoId)
        {
            var lista = new List<ProgresoLeccion>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_ListarProgresoPorEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = estudianteId;
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = (object?)cursoId ?? DBNull.Value;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new ProgresoLeccion
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EstudianteId = reader.GetInt32(reader.GetOrdinal("estudiante_id")),
                    LeccionId = reader.GetInt32(reader.GetOrdinal("leccion_id")),
                    LeccionTitulo = reader.GetString(reader.GetOrdinal("leccion_titulo")),
                    CursoId = reader.GetInt32(reader.GetOrdinal("curso_id")),
                    Completado = reader.GetBoolean(reader.GetOrdinal("completado")),
                    FechaCompletado = reader.IsDBNull(reader.GetOrdinal("fecha_completado")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha_completado"))
                });
            }

            return lista;
        }
    }
}
