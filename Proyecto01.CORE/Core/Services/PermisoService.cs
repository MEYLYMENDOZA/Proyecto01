using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class PermisoService : IPermisoService
    {
        private readonly IPermisoRepository _permisoRepository;

        public PermisoService(IPermisoRepository permisoRepository)
        {
            _permisoRepository = permisoRepository;
        }

        public async Task<IEnumerable<PermisoListDTO>> GetAll()
        {
            return await _permisoRepository.GetAll();
        }

        public async Task<PermisoResponseDTO?> GetById(int id)
        {
            return await _permisoRepository.GetById(id);
        }

        public async Task<int> Create(PermisoCreateDTO dto)
        {
            return await _permisoRepository.Insert(dto);
        }

        public async Task<int> Update(PermisoUpdateDTO dto)
        {
            return await _permisoRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _permisoRepository.Delete(id);
        }
    }
}
