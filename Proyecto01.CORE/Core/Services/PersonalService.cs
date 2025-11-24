using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class PersonalService : IPersonalService
    {
        private readonly IPersonalRepository _repository;

        public PersonalService(IPersonalRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PersonalListDTO>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<PersonalListDTO?> GetById(int id)
        {
            if (id <= 0) return null;
            return await _repository.GetById(id);
        }

        public async Task<PersonalListDTO?> GetByIdUsuario(int idUsuario)
        {
            if (idUsuario <= 0) return null;
            return await _repository.GetByIdUsuario(idUsuario);
        }

        public async Task<int> Insert(PersonalCreateDTO dto)
        {
            if (dto.IdUsuario <= 0)
                throw new ArgumentException("El ID del usuario es inválido.");

            return await _repository.Insert(dto);
        }

        public async Task<int> Update(PersonalUpdateDTO dto)
        {
            if (dto.IdPersonal <= 0)
                throw new ArgumentException("El ID del personal es inválido.");

            var exists = await _repository.Exists(dto.IdPersonal);
            if (!exists)
                throw new InvalidOperationException($"El personal con ID {dto.IdPersonal} no existe.");

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
