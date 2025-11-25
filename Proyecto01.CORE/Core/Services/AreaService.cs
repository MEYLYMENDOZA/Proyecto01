using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _repository;

        public AreaService(IAreaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AreaListDTO>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<AreaListDTO?> GetById(int id)
        {
            if (id <= 0) return null;
            return await _repository.GetById(id);
        }

        public async Task<int> Insert(AreaCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreArea))
                throw new ArgumentException("El nombre del área es requerido.");

            return await _repository.Insert(dto);
        }

        public async Task<int> Update(AreaUpdateDTO dto)
        {
            if (dto.IdArea <= 0)
                throw new ArgumentException("El ID del área es inválido.");

            if (string.IsNullOrWhiteSpace(dto.NombreArea))
                throw new ArgumentException("El nombre del área es requerido.");

            var exists = await _repository.Exists(dto.IdArea);
            if (!exists)
                throw new InvalidOperationException($"El área con ID {dto.IdArea} no existe.");

            return await _repository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            if (id <= 0) return false;

            var exists = await _repository.Exists(id);
            if (!exists) return false;

            return await _repository.Delete(id);
        }
    }
}
