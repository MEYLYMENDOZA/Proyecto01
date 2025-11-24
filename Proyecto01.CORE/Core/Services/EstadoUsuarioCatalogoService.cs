using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class EstadoUsuarioCatalogoService : IEstadoUsuarioCatalogoService
    {
        private readonly IEstadoUsuarioCatalogoRepository _estadoUsuarioCatalogoRepository;

        public EstadoUsuarioCatalogoService(IEstadoUsuarioCatalogoRepository estadoUsuarioCatalogoRepository)
        {
            _estadoUsuarioCatalogoRepository = estadoUsuarioCatalogoRepository;
        }

        public async Task<IEnumerable<EstadoUsuarioCatalogoListDTO>> GetAll()
        {
            return await _estadoUsuarioCatalogoRepository.GetAll();
        }

        public async Task<EstadoUsuarioCatalogoResponseDTO?> GetById(int id)
        {
            return await _estadoUsuarioCatalogoRepository.GetById(id);
        }

        public async Task<int> Create(EstadoUsuarioCatalogoCreateDTO dto)
        {
            return await _estadoUsuarioCatalogoRepository.Insert(dto);
        }

        public async Task<int> Update(EstadoUsuarioCatalogoUpdateDTO dto)
        {
            return await _estadoUsuarioCatalogoRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _estadoUsuarioCatalogoRepository.Delete(id);
        }
    }
}
