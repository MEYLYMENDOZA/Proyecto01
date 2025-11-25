using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class TipoSolicitudCatalogoService : ITipoSolicitudCatalogoService
    {
        private readonly ITipoSolicitudCatalogoRepository _tipoSolicitudCatalogoRepository;

        public TipoSolicitudCatalogoService(ITipoSolicitudCatalogoRepository tipoSolicitudCatalogoRepository)
        {
            _tipoSolicitudCatalogoRepository = tipoSolicitudCatalogoRepository;
        }

        public async Task<IEnumerable<TipoSolicitudCatalogoListDTO>> GetAll()
        {
            return await _tipoSolicitudCatalogoRepository.GetAll();
        }

        public async Task<TipoSolicitudCatalogoResponseDTO?> GetById(int id)
        {
            return await _tipoSolicitudCatalogoRepository.GetById(id);
        }

        public async Task<int> Create(TipoSolicitudCatalogoCreateDTO dto)
        {
            return await _tipoSolicitudCatalogoRepository.Insert(dto);
        }

        public async Task<int> Update(TipoSolicitudCatalogoUpdateDTO dto)
        {
            return await _tipoSolicitudCatalogoRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _tipoSolicitudCatalogoRepository.Delete(id);
        }
    }
}
