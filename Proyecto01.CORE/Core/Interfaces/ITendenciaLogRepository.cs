using Proyecto01.CORE.Core.Entities;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface ITendenciaLogRepository
    {
        Task<int> GuardarLog(PrediccionTendenciaLog log);
        Task<PrediccionTendenciaLog?> ObtenerUltimoLog(string tipoSla, int? idArea = null);
        Task<IEnumerable<PrediccionTendenciaLog>> ObtenerHistorial(
            string tipoSla,
            int? idArea = null,
            int limite = 12);
    }
}
