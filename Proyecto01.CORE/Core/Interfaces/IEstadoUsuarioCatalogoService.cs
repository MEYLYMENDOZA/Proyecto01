using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IEstadoUsuarioCatalogoService
    {
        Task<IEnumerable<EstadoUsuarioCatalogoListDTO>> GetAll();
        Task<EstadoUsuarioCatalogoResponseDTO?> GetById(int id);
        Task<int> Create(EstadoUsuarioCatalogoCreateDTO dto);
        Task<int> Update(EstadoUsuarioCatalogoUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
