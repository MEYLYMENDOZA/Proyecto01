using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IRolesSistemaService
    {
        Task<IEnumerable<RolesSistemaListDTO>> GetAll();
        Task<RolesSistemaResponseDTO?> GetById(int id);
        Task<int> Create(RolesSistemaCreateDTO dto);
        Task<int> Update(RolesSistemaUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
