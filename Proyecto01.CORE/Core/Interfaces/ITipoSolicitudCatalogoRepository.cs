using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface ITipoSolicitudCatalogoRepository
    {
        Task<IEnumerable<TipoSolicitudCatalogoListDTO>> GetAll();
        Task<TipoSolicitudCatalogoResponseDTO?> GetById(int id);
        Task<int> Insert(TipoSolicitudCatalogoCreateDTO dto);
        Task<int> Update(TipoSolicitudCatalogoUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
