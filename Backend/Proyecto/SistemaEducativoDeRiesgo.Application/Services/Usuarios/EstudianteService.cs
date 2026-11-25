using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Application.Services.Usuarios
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _repository;

        public EstudianteService(IEstudianteRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearEstudianteRequest request)
        {
            var estudiante = new Estudiante
            {
                UsuarioId = request.UsuarioId,
                CodigoEstudiante = request.CodigoEstudiante?.Trim(),
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                FechaNacimiento = request.FechaNacimiento,
                Telefono = request.Telefono?.Trim(),
                ContactoEmergencia = request.ContactoEmergencia?.Trim()
            };

            return _repository.InsertarAsync(estudiante);
        }

        public Task ActualizarAsync(int id, ActualizarEstudianteRequest request)
        {
            var estudiante = new Estudiante
            {
                Id = id,
                CodigoEstudiante = request.CodigoEstudiante?.Trim(),
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                FechaNacimiento = request.FechaNacimiento,
                Telefono = request.Telefono?.Trim(),
                ContactoEmergencia = request.ContactoEmergencia?.Trim()
            };

            return _repository.ActualizarAsync(estudiante);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<EstudianteResponse>> ListarAsync()
        {
            var estudiantes = await _repository.ListarAsync();
            return estudiantes.Select(e => new EstudianteResponse(
                e.Id, e.UsuarioId, e.CodigoEstudiante, e.Nombre, e.Apellido,
                e.FechaNacimiento, e.Telefono, e.ContactoEmergencia, e.CreadoEn
            )).ToList();
        }

        public async Task<EstudianteResponse?> ObtenerPorUsuarioIdAsync(int usuarioId)
        {
            var e = await _repository.ObtenerPorUsuarioIdAsync(usuarioId);
            if (e is null) return null;

            return new EstudianteResponse(
                e.Id, e.UsuarioId, e.CodigoEstudiante, e.Nombre, e.Apellido,
                e.FechaNacimiento, e.Telefono, e.ContactoEmergencia, e.CreadoEn);
        }
    }
}
