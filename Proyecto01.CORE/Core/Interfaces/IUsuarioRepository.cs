using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;

namespace Proyecto01.CORE.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<UsuarioResponseDTO>> GetAll();
        Task<UsuarioResponseDTO?> GetById(int id);
        Task<UsuarioResponseDTO?> GetByUsername(string username);
        Task<UsuarioResponseDTO?> GetByCorreo(string correo);
        Task<int> Insert(UsuarioCreateDTO dto);
        Task<bool> Update(Usuario usuario);
        Task<bool> Delete(int id); // Soft Delete
        Task<bool> HardDelete(int id); // Hard Delete (eliminación física)
        Task<bool> Exists(int id);
        Task<bool> ExistsByUsername(string username);
        Task<bool> ExistsByCorreo(string correo);
    }
}
