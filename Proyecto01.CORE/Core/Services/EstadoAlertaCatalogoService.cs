using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class EstadoAlertaCatalogoService : IEstadoAlertaCatalogoService
    {
        private readonly IEstadoAlertaCatalogoRepository _estadoAlertaCatalogoRepository;

        public EstadoAlertaCatalogoService(IEstadoAlertaCatalogoRepository estadoAlertaCatalogoRepository)
        {
            _estadoAlertaCatalogoRepository = estadoAlertaCatalogoRepository;
        }

        public async Task<IEnumerable<EstadoAlertaCatalogoListDTO>> GetAll()
        {
            return await _estadoAlertaCatalogoRepository.GetAll();
        }

        public async Task<EstadoAlertaCatalogoResponseDTO?> GetById(int id)
        {
            return await _estadoAlertaCatalogoRepository.GetById(id);
        }

        public async Task<int> Create(EstadoAlertaCatalogoCreateDTO dto)
        {
            return await _estadoAlertaCatalogoRepository.Insert(dto);
        }

        public async Task<int> Update(EstadoAlertaCatalogoUpdateDTO dto)
        {
            return await _estadoAlertaCatalogoRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _estadoAlertaCatalogoRepository.Delete(id);
        }
    }
}
