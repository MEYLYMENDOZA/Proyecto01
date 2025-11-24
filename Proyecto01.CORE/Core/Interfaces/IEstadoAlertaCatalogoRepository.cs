using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IEstadoAlertaCatalogoRepository
    {
        Task<IEnumerable<EstadoAlertaCatalogoListDTO>> GetAll();
        Task<EstadoAlertaCatalogoResponseDTO?> GetById(int id);
        Task<int> Insert(EstadoAlertaCatalogoCreateDTO dto);
        Task<int> Update(EstadoAlertaCatalogoUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
