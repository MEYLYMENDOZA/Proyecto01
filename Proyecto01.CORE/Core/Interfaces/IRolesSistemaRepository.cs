using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IRolesSistemaRepository
    {
        Task<IEnumerable<RolesSistemaListDTO>> GetAll();
        Task<RolesSistemaResponseDTO?> GetById(int id);
        Task<int> Insert(RolesSistemaCreateDTO dto);
        Task<int> Update(RolesSistemaUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
