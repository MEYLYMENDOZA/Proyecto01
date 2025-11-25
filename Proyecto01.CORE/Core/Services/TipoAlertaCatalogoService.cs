using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class TipoAlertaCatalogoService : ITipoAlertaCatalogoService
    {
        private readonly ITipoAlertaCatalogoRepository _tipoAlertaCatalogoRepository;

        public TipoAlertaCatalogoService(ITipoAlertaCatalogoRepository tipoAlertaCatalogoRepository)
        {
            _tipoAlertaCatalogoRepository = tipoAlertaCatalogoRepository;
        }

        public async Task<IEnumerable<TipoAlertaCatalogoListDTO>> GetAll()
        {
            return await _tipoAlertaCatalogoRepository.GetAll();
        }

        public async Task<TipoAlertaCatalogoResponseDTO?> GetById(int id)
        {
            return await _tipoAlertaCatalogoRepository.GetById(id);
        }

        public async Task<int> Create(TipoAlertaCatalogoCreateDTO dto)
        {
            return await _tipoAlertaCatalogoRepository.Insert(dto);
        }

        public async Task<int> Update(TipoAlertaCatalogoUpdateDTO dto)
        {
            return await _tipoAlertaCatalogoRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _tipoAlertaCatalogoRepository.Delete(id);
        }
    }
}
