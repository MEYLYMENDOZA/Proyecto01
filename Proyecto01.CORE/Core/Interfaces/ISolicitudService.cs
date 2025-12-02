using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface ISolicitudService
    {
        Task<IEnumerable<SolicitudListDTO>> GetAll();
        Task<SolicitudListDTO?> GetById(int id);
        Task<int> Insert(SolicitudCreateDTO dto);
        Task<int> Update(SolicitudUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<int> ProcesarSlas();
    }
}
