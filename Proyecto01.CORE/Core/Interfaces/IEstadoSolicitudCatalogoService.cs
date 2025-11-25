using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IEstadoSolicitudCatalogoService
    {
        Task<IEnumerable<EstadoSolicitudCatalogoListDTO>> GetAll();
        Task<EstadoSolicitudCatalogoResponseDTO?> GetById(int id);
        Task<int> Create(EstadoSolicitudCatalogoCreateDTO dto);
        Task<int> Update(EstadoSolicitudCatalogoUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
