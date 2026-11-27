using SistemaEducativoDeRiesgo.Domain.Entities.Comunicacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEducativoDeRiesgo.Domain.Interfaces.Comunicacion
{
    public interface IPublicacionForoRepository
    {

        Task<int> InsertarAsync(PublicacionForo publicacion);
        Task ActualizarAsync(int id, string contenido);
        Task EliminarAsync(int id);
        Task<List<PublicacionForo>> ListarPorForoAsync(int foroId);

    }
}
