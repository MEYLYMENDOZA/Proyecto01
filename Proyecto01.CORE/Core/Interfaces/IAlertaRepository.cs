using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IAlertaRepository
    {
        Task<IEnumerable<AlertaListDTO>> GetAll();
        Task<AlertaResponseDTO?> GetById(int id);
        Task<int> Insert(AlertaCreateDTO dto);
        Task<int> Update(AlertaUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
