using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface ITipoAlertaCatalogoService
    {
        Task<IEnumerable<TipoAlertaCatalogoListDTO>> GetAll();
        Task<TipoAlertaCatalogoResponseDTO?> GetById(int id);
        Task<int> Create(TipoAlertaCatalogoCreateDTO dto);
        Task<int> Update(TipoAlertaCatalogoUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
