using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Insfrasctuture.Persistence.Usuarios
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public EstudianteRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertarAsync(Estudiante estudiante)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_InsertarEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = estudiante.UsuarioId;
            command.Parameters.Add("@codigo_estudiante", SqlDbType.NVarChar, 50).Value = (object?)estudiante.CodigoEstudiante ?? DBNull.Value;
            command.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = estudiante.Nombre;
            command.Parameters.Add("@apellido", SqlDbType.NVarChar, 100).Value = estudiante.Apellido;
            command.Parameters.Add("@fecha_nacimiento", SqlDbType.Date).Value = estudiante.FechaNacimiento.ToDateTime(TimeOnly.MinValue);
            command.Parameters.Add("@telefono", SqlDbType.NVarChar, 20).Value = (object?)estudiante.Telefono ?? DBNull.Value;
            command.Parameters.Add("@contacto_emergencia", SqlDbType.NVarChar, 150).Value = (object?)estudiante.ContactoEmergencia ?? DBNull.Value;

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task ActualizarAsync(Estudiante estudiante)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ActualizarEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = estudiante.Id;
            command.Parameters.Add("@codigo_estudiante", SqlDbType.NVarChar, 50).Value = (object?)estudiante.CodigoEstudiante ?? DBNull.Value;
            command.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = estudiante.Nombre;
            command.Parameters.Add("@apellido", SqlDbType.NVarChar, 100).Value = estudiante.Apellido;
            command.Parameters.Add("@fecha_nacimiento", SqlDbType.Date).Value = estudiante.FechaNacimiento.ToDateTime(TimeOnly.MinValue);
            command.Parameters.Add("@telefono", SqlDbType.NVarChar, 20).Value = (object?)estudiante.Telefono ?? DBNull.Value;
            command.Parameters.Add("@contacto_emergencia", SqlDbType.NVarChar, 150).Value = (object?)estudiante.ContactoEmergencia ?? DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_EliminarEstudiante", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Estudiante>> ListarAsync()
        {
            var lista = new List<Estudiante>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ListarEstudiantes", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(MapEstudiante(reader));
            }

            return lista;
        }

        public async Task<Estudiante?> ObtenerPorUsuarioIdAsync(int usuarioId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("Usuarios.SP_ObtenerEstudiantePorId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = usuarioId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapEstudiante(reader) : null;
        }

        private static Estudiante MapEstudiante(SqlDataReader reader)
        {
            return new Estudiante
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                UsuarioId = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                CodigoEstudiante = reader.IsDBNull(reader.GetOrdinal("codigo_estudiante")) ? null : reader.GetString(reader.GetOrdinal("codigo_estudiante")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                FechaNacimiento = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento"))),
                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                ContactoEmergencia = reader.IsDBNull(reader.GetOrdinal("contacto_emergencia")) ? null : reader.GetString(reader.GetOrdinal("contacto_emergencia")),
                CreadoEn = reader.GetDateTime(reader.GetOrdinal("creado_en"))
            };
        }
    }
}
