using SistemaEducativoDeRiesgo.Application.DTOs.Configuracion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Application.Services.Configuracion
{
    public class ParametroSistemaService : IParametroSistemaService
    {
        private readonly IParametroSistemaRepository _parametroSistemaRepository;

        public ParametroSistemaService(IParametroSistemaRepository parametroSistemaRepository)
        {
            _parametroSistemaRepository = parametroSistemaRepository;
        }

        public Task<int> CrearAsync(CrearParametroSistemaRequest request)
        {
            var parametro = new ParametroSistema
            {
                Clave = request.Clave.Trim(),
                Valor = request.Valor.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                TipoDato = request.TipoDato?.Trim()
            };

            return _parametroSistemaRepository.InsertarAsync(parametro);
        }

        public Task ActualizarAsync(int id, ActualizarParametroSistemaRequest request)
        {
            var parametro = new ParametroSistema
            {
                Id = id,
                Valor = request.Valor.Trim(),
                Descripcion = request.Descripcion?.Trim(),
                TipoDato = request.TipoDato?.Trim()
            };

            return _parametroSistemaRepository.ActualizarAsync(parametro);
        }

        public Task EliminarAsync(int id) => _parametroSistemaRepository.EliminarAsync(id);

        public async Task<List<ParametroSistemaResponse>> ListarAsync()
        {
            var parametros = await _parametroSistemaRepository.ListarAsync();
            return parametros.Select(p => new ParametroSistemaResponse(
                p.Id,
                p.Clave,
                p.Valor,
                p.Descripcion,
                p.TipoDato,
                p.ActualizadoEn
            )).ToList();
        }
    }
}
