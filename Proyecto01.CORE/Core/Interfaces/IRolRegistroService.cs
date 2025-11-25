using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IRolRegistroService
    {
        Task<IEnumerable<RolRegistroListDTO>> GetAll();
        Task<RolRegistroResponseDTO?> GetById(int id);
        Task<int> Create(RolRegistroCreateDTO dto);
        Task<int> Update(RolRegistroUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
