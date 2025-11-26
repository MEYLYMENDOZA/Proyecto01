using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly Proyecto01DbContext _context;

        public UsuarioRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        // --- Getters de Datos Generales (Mantienen el mapeo sin Hash) ---

        public async Task<IEnumerable<UsuarioResponseDTO>> GetAll()
        {
            return await _context.Usuarios
                .Select(u => new UsuarioResponseDTO
                {
                    IdUsuario = u.IdUsuario,
                    Username = u.Username,
                    Correo = u.Correo,
                    IdRolSistema = u.IdRolSistema,
                    CreadoEn = u.CreadoEn
                })
                .ToListAsync();
        }

        public async Task<UsuarioResponseDTO?> GetById(int id)
        {
            return await _context.Usuarios
                .Where(u => u.IdUsuario == id)
                .Select(u => new UsuarioResponseDTO
                {
                    IdUsuario = u.IdUsuario,
                    Username = u.Username,
                    Correo = u.Correo,
                    IdRolSistema = u.IdRolSistema,
                    CreadoEn = u.CreadoEn
                })
                .FirstOrDefaultAsync();
        }


        // --- Getters para Seguridad (Necesitan el Hash) ---

        public async Task<UsuarioResponseDTO?> GetByUsername(string username)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null) return null;

            // Mapeo corregido: Incluye PasswordHash para que el Servicio pueda verificar
            return new UsuarioResponseDTO
            {
                IdUsuario = usuario.IdUsuario,
                Username = usuario.Username,
                Correo = usuario.Correo,
                PasswordHash = usuario.PasswordHash, // ¡CRÍTICO para seguridad!
                IdRolSistema = usuario.IdRolSistema,
                CreadoEn = usuario.CreadoEn
            };
        }

        public async Task<UsuarioResponseDTO?> GetByCorreo(string correo)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null) return null;

            // Mapeo corregido: Incluye PasswordHash para que el Servicio pueda verificar
            return new UsuarioResponseDTO
            {
                IdUsuario = usuario.IdUsuario,
                Username = usuario.Username,
                Correo = usuario.Correo,
                PasswordHash = usuario.PasswordHash, // ¡CRÍTICO para seguridad!
                IdRolSistema = usuario.IdRolSistema,
                CreadoEn = usuario.CreadoEn
            };
        }

        // --- CRUD Restante (Insert, Update, Delete, Exists) ---

        public async Task<int> Insert(UsuarioCreateDTO dto)
        {
            var usuario = new Usuario
            {
                Username = dto.Username,
                Correo = dto.Correo,
                PasswordHash = dto.PasswordHash,
                IdRolSistema = dto.IdRolSistema,
                IdEstadoUsuario = dto.IdEstadoUsuario,
                CreadoEn = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario.IdUsuario;
        }

        public async Task<bool> Update(Usuario usuario)
        {
            var existingUsuario = await _context.Usuarios.FindAsync(usuario.IdUsuario);
            if (existingUsuario == null) return false;

            // Actualizar los campos
            existingUsuario.Username = usuario.Username;
            existingUsuario.Correo = usuario.Correo;
            existingUsuario.IdRolSistema = usuario.IdRolSistema;
            existingUsuario.IdEstadoUsuario = usuario.IdEstadoUsuario;
            existingUsuario.ActualizadoEn = DateTime.UtcNow;

            // Si se proporciona un nuevo PasswordHash, actualizarlo
            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                existingUsuario.PasswordHash = usuario.PasswordHash;
            }

            _context.Usuarios.Update(existingUsuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.IdUsuario == id);
        }

        public async Task<bool> ExistsByUsername(string username)
        {
            return await _context.Usuarios.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByCorreo(string correo)
        {
            return await _context.Usuarios.AnyAsync(u => u.Correo == correo);
        }
    }
}