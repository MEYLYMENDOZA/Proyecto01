using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class RolesSistemaService : IRolesSistemaService
    {
        private readonly IRolesSistemaRepository _rolesSistemaRepository;

        public RolesSistemaService(IRolesSistemaRepository rolesSistemaRepository)
        {
            _rolesSistemaRepository = rolesSistemaRepository;
        }

        public async Task<IEnumerable<RolesSistemaListDTO>> GetAll()
        {
            return await _rolesSistemaRepository.GetAll();
        }

        public async Task<RolesSistemaResponseDTO?> GetById(int id)
        {
            return await _rolesSistemaRepository.GetById(id);
        }

        public async Task<int> Create(RolesSistemaCreateDTO dto)
        {
            return await _rolesSistemaRepository.Insert(dto);
        }

        public async Task<int> Update(RolesSistemaUpdateDTO dto)
        {
            return await _rolesSistemaRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _rolesSistemaRepository.Delete(id);
        }
    }
}
