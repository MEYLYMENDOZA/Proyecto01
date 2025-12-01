using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResponseDTO>> GetAll();
        Task<UsuarioResponseDTO?> GetById(int id);
        Task<UsuarioResponseDTO?> SignIn(string username, string password);
        Task<int> SignUp(UsuarioCreateDTO dto);
        Task<bool> Update(UsuarioUpdateDTO dto);
        Task<bool> Delete(int id);
    }
}
