using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IPersonalRepository
    {
        Task<IEnumerable<PersonalListDTO>> GetAll();
        Task<PersonalListDTO?> GetById(int id);
        Task<PersonalListDTO?> GetByIdUsuario(int idUsuario);
        Task<int> Insert(PersonalCreateDTO dto);
        Task<int> Update(PersonalUpdateDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
    }
}
