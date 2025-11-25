using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteService(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<IEnumerable<ReporteListDTO>> GetAll()
        {
            return await _reporteRepository.GetAll();
        }

        public async Task<ReporteResponseDTO?> GetById(int id)
        {
            return await _reporteRepository.GetById(id);
        }

        public async Task<int> Create(ReporteCreateDTO dto)
        {
            return await _reporteRepository.Insert(dto);
        }

        public async Task<int> Update(ReporteUpdateDTO dto)
        {
            return await _reporteRepository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            return await _reporteRepository.Delete(id);
        }
    }
}
