using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<AreaListDTO>> GetAll();
        Task<AreaListDTO?> GetById(int id);
        Task<int> Insert(AreaCreateDTO dto);
        Task<int> Update(AreaUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
