using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IConfigSlaRepository
    {
        Task<IEnumerable<ConfigSlaListDTO>> GetAll();
        Task<ConfigSlaResponseDTO?> GetById(int id);
        Task<int> Insert(ConfigSlaCreateDTO dto);
        Task<bool> Update(ConfigSlaUpdateDTO dto); // <-- Cambiado de Task<int> a Task<bool>
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
