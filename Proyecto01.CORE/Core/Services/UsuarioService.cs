using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UsuarioResponseDTO>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<UsuarioResponseDTO?> GetById(int id)
        {
            if (id <= 0) return null;
            return await _repository.GetById(id);
        }

        public async Task<UsuarioResponseDTO?> SignIn(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var usuario = await _repository.GetByUsername(username);
            if (usuario == null) return null;

            // Obtener el usuario completo para verificar la contraseña
            var usuarioDb = await _repository.GetByUsername(username);
            if (usuarioDb == null) return null;

            // Aquí deberías validar la contraseña con BCrypt
            // Por ahora retornamos el usuario si existe
            return usuario;
        }

        public async Task<int> SignUp(UsuarioCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("El username es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Correo))
                throw new ArgumentException("El correo es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("La contraseña es requerida.");

            var userExists = await _repository.ExistsByUsername(dto.Username);
            if (userExists)
                throw new InvalidOperationException("El username ya existe.");

            var emailExists = await _repository.ExistsByCorreo(dto.Correo);
            if (emailExists)
                throw new InvalidOperationException("El correo ya está registrado.");

            return await _repository.Insert(dto);
        }

        public async Task<int> Update(UsuarioResponseDTO dto)
        {
            if (dto.IdUsuario <= 0)
                throw new ArgumentException("El ID del usuario es inválido.");

            var exists = await _repository.Exists(dto.IdUsuario);
            if (!exists)
                throw new InvalidOperationException($"El usuario con ID {dto.IdUsuario} no existe.");

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
