using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class AlertaService : IAlertaService
    {
        private readonly IAlertaRepository _alertaRepository;

        public AlertaService(IAlertaRepository alertaRepository)
        {
            _alertaRepository = alertaRepository;
        }

        public async Task<IEnumerable<AlertaListDTO>> GetAll()
        {
            return await _alertaRepository.GetAll();
        }

        public async Task<AlertaResponseDTO?> GetById(int id)
        {
            return await _alertaRepository.GetById(id);
        }

        public async Task<int> Create(AlertaCreateDTO dto)
        {
            return await _alertaRepository.Insert(dto);
        }

        public async Task<int> Update(AlertaUpdateDTO dto)
        {
            return await _alertaRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _alertaRepository.Delete(id);
        }
    }
}
