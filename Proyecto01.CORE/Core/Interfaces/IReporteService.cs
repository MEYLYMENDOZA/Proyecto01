using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IReporteService
    {
        Task<IEnumerable<ReporteListDTO>> GetAll();
        Task<ReporteResponseDTO?> GetById(int id);
        Task<int> Create(ReporteCreateDTO dto);
        Task<int> Update(ReporteUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
