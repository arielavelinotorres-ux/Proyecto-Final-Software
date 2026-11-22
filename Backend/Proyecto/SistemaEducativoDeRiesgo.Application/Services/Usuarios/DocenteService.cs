using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Usuarios;
using SistemaEducativoDeRiesgo.Application.Interfaces.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Entities.Usuarios;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Usuario;

namespace SistemaEducativoDeRiesgo.Application.Services.Usuarios
{
    public class DocenteService : IDocenteService
    {
        private readonly IDocenteRepository _repository;

        public DocenteService(IDocenteRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearDocenteRequest request)
        {
            var docente = new Docente
            {
                UsuarioId = request.UsuarioId,
                CodigoDocente = request.CodigoDocente?.Trim(),
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Especialidad = request.Especialidad?.Trim(),
                Telefono = request.Telefono?.Trim(),
                Biografia = request.Biografia?.Trim()
            };

            return _repository.InsertarAsync(docente);
        }

        public Task ActualizarAsync(int id, ActualizarDocenteRequest request)
        {
            var docente = new Docente
            {
                Id = id,
                CodigoDocente = request.CodigoDocente?.Trim(),
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Especialidad = request.Especialidad?.Trim(),
                Telefono = request.Telefono?.Trim(),
                Biografia = request.Biografia?.Trim()
            };

            return _repository.ActualizarAsync(docente);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<DocenteResponse>> ListarAsync()
        {
            var docentes = await _repository.ListarAsync();
            return docentes.Select(d => new DocenteResponse(
                d.Id, d.UsuarioId, d.CodigoDocente, d.Nombre, d.Apellido,
                d.Especialidad, d.Telefono, d.Biografia, d.CreadoEn
            )).ToList();
        }
    }
}
