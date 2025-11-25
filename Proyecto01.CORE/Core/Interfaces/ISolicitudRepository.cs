using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface ISolicitudRepository
    {
        Task<IEnumerable<SolicitudListDTO>> GetAll();
        Task<SolicitudListDTO?> GetById(int id);
        Task<int> Insert(SolicitudCreateDTO dto);
        Task<int> Update(SolicitudUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
