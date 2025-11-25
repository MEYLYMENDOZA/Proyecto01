using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class EstadoSolicitudCatalogoService : IEstadoSolicitudCatalogoService
    {
        private readonly IEstadoSolicitudCatalogoRepository _estadoSolicitudCatalogoRepository;

        public EstadoSolicitudCatalogoService(IEstadoSolicitudCatalogoRepository estadoSolicitudCatalogoRepository)
        {
            _estadoSolicitudCatalogoRepository = estadoSolicitudCatalogoRepository;
        }

        public async Task<IEnumerable<EstadoSolicitudCatalogoListDTO>> GetAll()
        {
            return await _estadoSolicitudCatalogoRepository.GetAll();
        }

        public async Task<EstadoSolicitudCatalogoResponseDTO?> GetById(int id)
        {
            return await _estadoSolicitudCatalogoRepository.GetById(id);
        }

        public async Task<int> Create(EstadoSolicitudCatalogoCreateDTO dto)
        {
            return await _estadoSolicitudCatalogoRepository.Insert(dto);
        }

        public async Task<int> Update(EstadoSolicitudCatalogoUpdateDTO dto)
        {
            return await _estadoSolicitudCatalogoRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _estadoSolicitudCatalogoRepository.Delete(id);
        }
    }
}
