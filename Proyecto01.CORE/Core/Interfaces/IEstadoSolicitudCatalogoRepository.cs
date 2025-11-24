using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IEstadoSolicitudCatalogoRepository
    {
        Task<IEnumerable<EstadoSolicitudCatalogoListDTO>> GetAll();
        Task<EstadoSolicitudCatalogoResponseDTO?> GetById(int id);
        Task<int> Insert(EstadoSolicitudCatalogoCreateDTO dto);
        Task<int> Update(EstadoSolicitudCatalogoUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
