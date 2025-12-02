using Proyecto01.CORE.Core.Entities;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface ISlaRepository
    {
        Task<IEnumerable<Solicitud>> ObtenerSolicitudesPorSla(
            string tipoSla,
            int? mes = null,
            int? anio = null,
            int? idArea = null);
        
        Task<int> ObtenerDiasUmbralSla(string tipoSla);
    }
}
