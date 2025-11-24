using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IPermisoService
    {
        Task<IEnumerable<PermisoListDTO>> GetAll();
        Task<PermisoResponseDTO?> GetById(int id);
        Task<int> Create(PermisoCreateDTO dto);
        Task<int> Update(PermisoUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
