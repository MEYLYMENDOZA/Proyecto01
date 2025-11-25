using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IConfigSlaService
    {
        Task<IEnumerable<ConfigSlaListDTO>> GetAll();
        Task<ConfigSlaResponseDTO?> GetById(int id);
        Task<int> Create(ConfigSlaCreateDTO dto);
        Task<int> Update(ConfigSlaUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
