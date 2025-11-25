using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class RolRegistroService : IRolRegistroService
    {
        private readonly IRolRegistroRepository _rolRegistroRepository;

        public RolRegistroService(IRolRegistroRepository rolRegistroRepository)
        {
            _rolRegistroRepository = rolRegistroRepository;
        }

        public async Task<IEnumerable<RolRegistroListDTO>> GetAll()
        {
            return await _rolRegistroRepository.GetAll();
        }

        public async Task<RolRegistroResponseDTO?> GetById(int id)
        {
            return await _rolRegistroRepository.GetById(id);
        }

        public async Task<int> Create(RolRegistroCreateDTO dto)
        {
            return await _rolRegistroRepository.Insert(dto);
        }

        public async Task<int> Update(RolRegistroUpdateDTO dto)
        {
            return await _rolRegistroRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _rolRegistroRepository.Delete(id);
        }
    }
}
