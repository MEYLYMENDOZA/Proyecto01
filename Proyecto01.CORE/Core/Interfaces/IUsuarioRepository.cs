using Proyecto01.CORE.Core.DTOs;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<UsuarioResponseDTO>> GetAll();
        Task<UsuarioResponseDTO?> GetById(int id);
        Task<UsuarioResponseDTO?> GetByUsername(string username);
        Task<int> Insert(UsuarioCreateDTO dto);
        Task<int> Update(UsuarioResponseDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
        Task<bool> ExistsByUsername(string username);
        Task<bool> ExistsByCorreo(string correo);
    }
}
