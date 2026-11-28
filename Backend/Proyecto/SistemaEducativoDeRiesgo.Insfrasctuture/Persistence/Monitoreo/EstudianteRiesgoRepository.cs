using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo;


namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Monitoreo
{
    public class EstudianteRiesgoRepository : IEstudianteRiesgoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public EstudianteRiesgoRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(EstudianteRiesgo registro)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_InsertarEstudianteRiesgo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = registro.EstudianteId;
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = registro.CursoId;
            command.Parameters.Add("@nivel_riesgo", SqlDbType.NVarChar, 20).Value = registro.NivelRiesgo;
            command.Parameters.Add("@motivo", SqlDbType.NVarChar, 255).Value = registro.Motivo;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(EstudianteRiesgo registro)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_ActualizarEstudianteRiesgo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = registro.Id;
            command.Parameters.Add("@nivel_riesgo", SqlDbType.NVarChar, 20).Value = registro.NivelRiesgo;
            command.Parameters.Add("@motivo", SqlDbType.NVarChar, 255).Value = registro.Motivo;
            command.Parameters.Add("@atendido", SqlDbType.Bit).Value = registro.Atendido;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_EliminarEstudianteRiesgo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<EstudianteRiesgo>> ListarAsync(int? cursoId)
        {
            var lista = new List<EstudianteRiesgo>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_ListarEstudiantesRiesgo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = (object?)cursoId ?? DBNull.Value;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new EstudianteRiesgo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EstudianteId = reader.GetInt32(reader.GetOrdinal("estudiante_id")),
                    EstudianteNombre = reader.GetString(reader.GetOrdinal("estudiante_nombre")),
                    CursoId = reader.GetInt32(reader.GetOrdinal("curso_id")),
                    CursoTitulo = reader.GetString(reader.GetOrdinal("curso_titulo")),
                    NivelRiesgo = reader.GetString(reader.GetOrdinal("nivel_riesgo")),
                    Motivo = reader.GetString(reader.GetOrdinal("motivo")),
                    DetectadoEn = reader.GetDateTime(reader.GetOrdinal("detectado_en")),
                    Atendido = reader.GetBoolean(reader.GetOrdinal("atendido"))
                });
            }

            return lista;
        }
    }
}
