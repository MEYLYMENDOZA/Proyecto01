using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IEstadoUsuarioCatalogoRepository
    {
        Task<IEnumerable<EstadoUsuarioCatalogoListDTO>> GetAll();
        Task<EstadoUsuarioCatalogoResponseDTO?> GetById(int id);
        Task<int> Insert(EstadoUsuarioCatalogoCreateDTO dto);
        Task<int> Update(EstadoUsuarioCatalogoUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
