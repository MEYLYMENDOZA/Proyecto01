using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Core.Entities;

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

        public async Task<UsuarioResponseDTO?> SignIn(string correo, string password)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password))
                return null;

            // Buscar usuario por correo
            var usuario = await _repository.GetByCorreo(correo);
            if (usuario == null) return null;

            // Verificar que el usuario esté activo (IdEstadoUsuario = 1)
            if (usuario.IdEstadoUsuario != 1)
                return null; // Usuario inactivo o bloqueado

            // Verificar la contraseña usando BCrypt
            if (string.IsNullOrEmpty(usuario.PasswordHash))
                return null;

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            if (!isValidPassword)
                return null;

            // No retornar el PasswordHash al cliente por seguridad
            usuario.PasswordHash = null;
            return usuario;
        }

        public async Task<int> SignUp(UsuarioCreateDTO dto)
        {
            // ========================================
            // VALIDACIONES DE CAMPOS USUARIO
            // ========================================
            
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("El username es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Correo))
                throw new ArgumentException("El correo es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("La contraseña es requerida.");

            // Validar que IdRolSistema sea válido
            if (dto.IdRolSistema <= 0)
                throw new ArgumentException("Debe especificar un rol de sistema válido.");

            // ========================================
            // VALIDACIONES DE CAMPOS PERSONAL
            // ========================================
            
            if (string.IsNullOrWhiteSpace(dto.Nombres))
                throw new ArgumentException("El nombre es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Apellidos))
                throw new ArgumentException("El apellido es requerido.");

            // ========================================
            // VALIDACIONES DE DUPLICADOS
            // ========================================
            
            var userExists = await _repository.ExistsByUsername(dto.Username);
            if (userExists)
                throw new InvalidOperationException("El username ya existe.");

            var emailExists = await _repository.ExistsByCorreo(dto.Correo);
            if (emailExists)
                throw new InvalidOperationException("El correo ya está registrado.");

            // ========================================
            // PREPARAR DATOS PARA GUARDAR
            // ========================================
            
            // Generar hash de la contraseña usando BCrypt
            dto.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Asegurar que IdEstadoUsuario sea 1 (ACTIVO)
            dto.IdEstadoUsuario = 1;

            // Guardar Usuario y Personal en una transacción
            return await _repository.Insert(dto);
        }

        public async Task<bool> Update(UsuarioUpdateDTO dto)
        {
            if (dto.IdUsuario <= 0)
                throw new ArgumentException("El ID del usuario es inválido.");

            // Verificar que el usuario existe
            var usuarioActual = await _repository.GetById(dto.IdUsuario);
            if (usuarioActual == null)
                throw new InvalidOperationException($"El usuario con ID {dto.IdUsuario} no existe.");

            // Validar que IdRolSistema sea válido
            if (dto.IdRolSistema <= 0)
                throw new ArgumentException("Debe especificar un rol de sistema válido.");

            // Validar que IdEstadoUsuario sea válido
            if (dto.IdEstadoUsuario <= 0)
                throw new ArgumentException("Debe especificar un estado de usuario válido.");

            // Verificar que el username no esté en uso por otro usuario
            var usuarioConMismoUsername = await _repository.GetByUsername(dto.Username);
            if (usuarioConMismoUsername != null && usuarioConMismoUsername.IdUsuario != dto.IdUsuario)
                throw new InvalidOperationException("El username ya está en uso por otro usuario.");

            // Verificar que el correo no esté en uso por otro usuario
            var usuarioConMismoCorreo = await _repository.GetByCorreo(dto.Correo);
            if (usuarioConMismoCorreo != null && usuarioConMismoCorreo.IdUsuario != dto.IdUsuario)
                throw new InvalidOperationException("El correo ya está en uso por otro usuario.");

            // Crear la entidad Usuario con los datos actualizados
            var usuario = new Usuario
            {
                IdUsuario = dto.IdUsuario,
                Username = dto.Username,
                Correo = dto.Correo,
                IdRolSistema = dto.IdRolSistema,
                IdEstadoUsuario = dto.IdEstadoUsuario,
                PasswordHash = null // Por defecto no se actualiza
            };

            // Si se proporciona una nueva contraseña, hashearla
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            return await _repository.Update(usuario);
        }

        // SOFT DELETE - Marca el usuario como inactivo
        // No elimina físicamente el registro para mantener la integridad referencial
        public async Task<bool> Delete(int id)
        {
            if (id <= 0) return false;

            var exists = await _repository.Exists(id);
            if (!exists) return false;

            // Este método marca al usuario como INACTIVO (soft delete)
            return await _repository.Delete(id);
        }
    }
}
