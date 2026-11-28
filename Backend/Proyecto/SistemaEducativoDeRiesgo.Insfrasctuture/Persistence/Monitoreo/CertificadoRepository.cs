using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Monitoreo
{
    public class CertificadoRepository : ICertificadoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public CertificadoRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(Certificado certificado)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_InsertarCertificado", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = certificado.EstudianteId;
            command.Parameters.Add("@curso_id", SqlDbType.Int).Value = certificado.CursoId;
            command.Parameters.Add("@codigo_validacion", SqlDbType.NVarChar, 50).Value = certificado.CodigoValidacion;
            command.Parameters.Add("@url_pdf", SqlDbType.NVarChar, 255).Value = (object?)certificado.UrlPdf ?? DBNull.Value;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(Certificado certificado)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_ActualizarCertificado", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = certificado.Id;
            command.Parameters.Add("@codigo_validacion", SqlDbType.NVarChar, 50).Value = certificado.CodigoValidacion;
            command.Parameters.Add("@url_pdf", SqlDbType.NVarChar, 255).Value = (object?)certificado.UrlPdf ?? DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<Certificado?> ValidarPorCodigoAsync(string codigoValidacion)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_ValidarCertificadoPorCodigo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@codigo_validacion", SqlDbType.NVarChar, 50).Value = codigoValidacion;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new Certificado
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                CodigoValidacion = reader.GetString(reader.GetOrdinal("codigo_validacion")),
                FechaEmision = reader.GetDateTime(reader.GetOrdinal("fecha_emision")),
                UrlPdf = reader.IsDBNull(reader.GetOrdinal("url_pdf")) ? null : reader.GetString(reader.GetOrdinal("url_pdf")),
                EstudianteNombre = reader.GetString(reader.GetOrdinal("estudiante_nombre")),
                CursoTitulo = reader.GetString(reader.GetOrdinal("curso_titulo"))
            };
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_EliminarCertificado", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Certificado>> ListarPorEstudianteAsync(int estudianteId)
        {
            var lista = new List<Certificado>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Monitoreo.SP_ListarCertificadosPorEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@estudiante_id", SqlDbType.Int).Value = estudianteId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Certificado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    EstudianteId = reader.GetInt32(reader.GetOrdinal("estudiante_id")),
                    CursoId = reader.GetInt32(reader.GetOrdinal("curso_id")),
                    CursoTitulo = reader.GetString(reader.GetOrdinal("curso_titulo")),
                    CodigoValidacion = reader.GetString(reader.GetOrdinal("codigo_validacion")),
                    FechaEmision = reader.GetDateTime(reader.GetOrdinal("fecha_emision")),
                    UrlPdf = reader.IsDBNull(reader.GetOrdinal("url_pdf")) ? null : reader.GetString(reader.GetOrdinal("url_pdf"))
                });
            }

            return lista;
        }
    }
}
