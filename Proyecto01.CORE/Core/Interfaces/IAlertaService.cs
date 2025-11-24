using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IAlertaService
    {
        Task<IEnumerable<AlertaListDTO>> GetAll();
        Task<AlertaResponseDTO?> GetById(int id);
        Task<int> Create(AlertaCreateDTO dto);
        Task<int> Update(AlertaUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
