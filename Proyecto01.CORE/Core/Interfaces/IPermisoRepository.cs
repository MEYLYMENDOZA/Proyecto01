using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IPermisoRepository
    {
        Task<IEnumerable<PermisoListDTO>> GetAll();
        Task<PermisoResponseDTO?> GetById(int id);
        Task<int> Insert(PermisoCreateDTO dto);
        Task<int> Update(PermisoUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
