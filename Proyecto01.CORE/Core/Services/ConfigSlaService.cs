using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class ConfigSlaService : IConfigSlaService
    {
        private readonly IConfigSlaRepository _configSlaRepository;

        public ConfigSlaService(IConfigSlaRepository configSlaRepository)
        {
            _configSlaRepository = configSlaRepository;
        }

        public async Task<IEnumerable<ConfigSlaListDTO>> GetAll()
        {
            return await _configSlaRepository.GetAll();
        }

        public async Task<ConfigSlaResponseDTO?> GetById(int id)
        {
            return await _configSlaRepository.GetById(id);
        }

        public async Task<int> Create(ConfigSlaCreateDTO dto)
        {
            return await _configSlaRepository.Insert(dto);
        }

        public async Task<bool> Update(ConfigSlaUpdateDTO dto)
        {
            return await _configSlaRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _configSlaRepository.Delete(id);
        }
    }
}
