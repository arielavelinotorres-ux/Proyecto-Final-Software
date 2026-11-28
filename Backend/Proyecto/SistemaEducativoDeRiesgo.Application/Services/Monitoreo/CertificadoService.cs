using System;
using System.Collections.Generic;
using System.Text;
using SistemaEducativoDeRiesgo.Application.DTOs.Monitoreo;
using SistemaEducativoDeRiesgo.Application.Interfaces.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Entities.Monitoreo;
using SistemaEducativoDeRiesgo.Domain.Interfaces.Monitoreo;


namespace SistemaEducativoDeRiesgo.Application.Services.Monitoreo
{
    public class CertificadoService : ICertificadoService
    {
        private readonly ICertificadoRepository _repository;

        public CertificadoService(ICertificadoRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CrearAsync(CrearCertificadoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CodigoValidacion))
                throw new ArgumentException("El código de validación es obligatorio.");

            var certificado = new Certificado
            {
                EstudianteId = request.EstudianteId,
                CursoId = request.CursoId,
                CodigoValidacion = request.CodigoValidacion.Trim(),
                UrlPdf = string.IsNullOrWhiteSpace(request.UrlPdf) ? null : request.UrlPdf.Trim()
            };

            return _repository.InsertarAsync(certificado);
        }

        public Task ActualizarAsync(int id, ActualizarCertificadoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CodigoValidacion))
                throw new ArgumentException("El código de validación no puede estar vacío.");

            var certificado = new Certificado
            {
                Id = id,
                CodigoValidacion = request.CodigoValidacion.Trim(),
                UrlPdf = string.IsNullOrWhiteSpace(request.UrlPdf) ? null : request.UrlPdf.Trim()
            };

            return _repository.ActualizarAsync(certificado);
        }

        public async Task<ValidarCertificadoResponse?> ValidarPorCodigoAsync(string codigoValidacion)
        {
            if (string.IsNullOrWhiteSpace(codigoValidacion))
                throw new ArgumentException("El código de validación es obligatorio.");

            var c = await _repository.ValidarPorCodigoAsync(codigoValidacion.Trim());
            if (c is null) return null;

            return new ValidarCertificadoResponse(
                c.Id,
                c.CodigoValidacion,
                c.FechaEmision,
                c.UrlPdf,
                c.EstudianteNombre ?? string.Empty,
                c.CursoTitulo ?? string.Empty);
        }

        public Task EliminarAsync(int id) => _repository.EliminarAsync(id);

        public async Task<List<CertificadoResponse>> ListarPorEstudianteAsync(int estudianteId)
        {
            var lista = await _repository.ListarPorEstudianteAsync(estudianteId);
            return lista.Select(c => new CertificadoResponse(
                c.Id,
                c.EstudianteId,
                c.CursoId,
                c.CursoTitulo ?? string.Empty,
                c.CodigoValidacion,
                c.FechaEmision,
                c.UrlPdf
            )).ToList();
        }
    }
}
