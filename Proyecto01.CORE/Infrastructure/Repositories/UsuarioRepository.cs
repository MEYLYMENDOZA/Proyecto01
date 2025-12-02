using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly Proyecto01DbContext _context;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(Proyecto01DbContext context, ILogger<UsuarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // --- Getters de Datos Generales (Mantienen el mapeo sin Hash) ---

        public async Task<IEnumerable<UsuarioResponseDTO>> GetAll()
        {
            try
            {
                _logger.LogInformation("UsuarioRepository.GetAll() - Iniciando consulta");
                
                var usuarios = await _context.Usuarios
                    .Select(u => new UsuarioResponseDTO
                    {
                        IdUsuario = u.IdUsuario,
                        Username = u.Username,
                        Correo = u.Correo,
                        IdRolSistema = u.IdRolSistema,
                        IdEstadoUsuario = u.IdEstadoUsuario,
                        CreadoEn = u.CreadoEn
                    })
                    .ToListAsync();

                _logger.LogInformation($"UsuarioRepository.GetAll() - Consulta exitosa, {usuarios.Count} registros encontrados");
                return usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UsuarioRepository.GetAll() - Error al obtener usuarios");
                throw;
            }
        }

        public async Task<UsuarioResponseDTO?> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"UsuarioRepository.GetById({id}) - Iniciando consulta");

                var usuario = await _context.Usuarios
                    .Where(u => u.IdUsuario == id)
                    .Select(u => new UsuarioResponseDTO
                    {
                        IdUsuario = u.IdUsuario,
                        Username = u.Username,
                        Correo = u.Correo,
                        IdRolSistema = u.IdRolSistema,
                        IdEstadoUsuario = u.IdEstadoUsuario,
                        CreadoEn = u.CreadoEn
                    })
                    .FirstOrDefaultAsync();

                _logger.LogInformation($"UsuarioRepository.GetById({id}) - Consulta exitosa, usuario encontrado: {usuario != null}");
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.GetById({id}) - Error al obtener usuario");
                throw;
            }
        }


        // --- Getters para Seguridad (Necesitan el Hash) ---

        public async Task<UsuarioResponseDTO?> GetByUsername(string username)
        {
            try
            {
                _logger.LogInformation($"UsuarioRepository.GetByUsername('{username}') - Iniciando consulta");

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (usuario == null)
                {
                    _logger.LogInformation($"UsuarioRepository.GetByUsername('{username}') - Usuario no encontrado");
                    return null;
                }

                var resultado = new UsuarioResponseDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    Username = usuario.Username,
                    Correo = usuario.Correo,
                    PasswordHash = usuario.PasswordHash,
                    IdRolSistema = usuario.IdRolSistema,
                    IdEstadoUsuario = usuario.IdEstadoUsuario,
                    CreadoEn = usuario.CreadoEn
                };

                _logger.LogInformation($"UsuarioRepository.GetByUsername('{username}') - Usuario encontrado (ID: {usuario.IdUsuario})");
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.GetByUsername('{username}') - Error al obtener usuario");
                throw;
            }
        }

        public async Task<UsuarioResponseDTO?> GetByCorreo(string correo)
        {
            try
            {
                _logger.LogInformation($"UsuarioRepository.GetByCorreo('{correo}') - Iniciando consulta");

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Correo == correo);

                if (usuario == null)
                {
                    _logger.LogInformation($"UsuarioRepository.GetByCorreo('{correo}') - Usuario no encontrado");
                    return null;
                }

                var resultado = new UsuarioResponseDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    Username = usuario.Username,
                    Correo = usuario.Correo,
                    PasswordHash = usuario.PasswordHash,
                    IdRolSistema = usuario.IdRolSistema,
                    IdEstadoUsuario = usuario.IdEstadoUsuario,
                    CreadoEn = usuario.CreadoEn
                };

                _logger.LogInformation($"UsuarioRepository.GetByCorreo('{correo}') - Usuario encontrado (ID: {usuario.IdUsuario})");
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.GetByCorreo('{correo}') - Error al obtener usuario");
                throw;
            }
        }

        // --- CRUD Restante (Insert, Update, Delete, Exists) ---

        // MÉTODO MEJORADO: Crea Usuario Y Personal en una transacción
        public async Task<int> Insert(UsuarioCreateDTO dto)
        {
            try
            {
                _logger.LogInformation($"UsuarioRepository.Insert() - Iniciando creación de usuario y personal para: {dto.Username}");

                // 1. CREAR LA ENTIDAD USUARIO
                var usuario = new Usuario
                {
                    Username = dto.Username,
                    Correo = dto.Correo,
                    PasswordHash = dto.PasswordHash, // Ya hasheado en el Service
                    IdRolSistema = dto.IdRolSistema,
                    IdEstadoUsuario = dto.IdEstadoUsuario, // Asignado por el Service (default 1)
                    CreadoEn = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuario);
                _logger.LogInformation($"UsuarioRepository.Insert() - Usuario agregado a contexto (sin guardar)");

                // Guardar para obtener el IdUsuario autoincremental
                await _context.SaveChangesAsync();
                _logger.LogInformation($"UsuarioRepository.Insert() - Usuario guardado exitosamente con ID: {usuario.IdUsuario}");

                // 2. CREAR LA ENTIDAD PERSONAL vinculada al Usuario recién creado
                var personal = new Personal
                {
                    IdUsuario = usuario.IdUsuario, // ? Usar el ID generado automáticamente
                    Nombres = dto.Nombres,
                    Apellidos = dto.Apellidos,
                    Documento = dto.Documento,
                    Estado = "Activo", // Estado por defecto
                    CreadoEn = DateTime.UtcNow
                };

                _context.Personales.Add(personal);
                _logger.LogInformation($"UsuarioRepository.Insert() - Personal agregado a contexto (sin guardar)");

                // Guardar Personal
                await _context.SaveChangesAsync();
                _logger.LogInformation($"UsuarioRepository.Insert() - Personal guardado exitosamente con ID: {personal.IdPersonal}");

                _logger.LogInformation($"UsuarioRepository.Insert() - Usuario ({usuario.IdUsuario}) y Personal ({personal.IdPersonal}) creados exitosamente");
                return usuario.IdUsuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.Insert() - Error al crear usuario y personal: {dto.Username}");
                throw;
            }
        }

        public async Task<bool> Update(Usuario usuario)
        {
            try
            {
                _logger.LogInformation($"UsuarioRepository.Update() - Actualizando usuario ID: {usuario.IdUsuario}");

                var existingUsuario = await _context.Usuarios.FindAsync(usuario.IdUsuario);
                if (existingUsuario == null)
                {
                    _logger.LogWarning($"UsuarioRepository.Update() - Usuario ID {usuario.IdUsuario} no encontrado");
                    return false;
                }

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

                _logger.LogInformation($"UsuarioRepository.Update() - Usuario ID {usuario.IdUsuario} actualizado exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.Update() - Error al actualizar usuario ID: {usuario.IdUsuario}");
                throw;
            }
        }

        // SOFT DELETE - Marca el usuario como INACTIVO en lugar de eliminarlo físicamente
        public async Task<bool> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"UsuarioRepository.Delete() - Iniciando soft delete para usuario ID: {id}");

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning($"UsuarioRepository.Delete() - Usuario ID {id} no encontrado");
                    return false;
                }

                // Buscar el ID del estado "INACTIVO" o "BLOQUEADO"
                var estadoInactivo = await _context.EstadosUsuario
                    .Where(e => e.Codigo == "INACTIVO" || e.Codigo == "BLOQUEADO")
                    .Select(e => e.IdEstadoUsuario)
                    .FirstOrDefaultAsync();

                if (estadoInactivo == 0)
                {
                    _logger.LogWarning("UsuarioRepository.Delete() - Estado INACTIVO no encontrado en BD, usando valor por defecto 2");
                    estadoInactivo = 2;
                }

                // Soft delete: cambiar estado a INACTIVO en lugar de eliminar
                usuario.IdEstadoUsuario = estadoInactivo;
                usuario.ActualizadoEn = DateTime.UtcNow;

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"UsuarioRepository.Delete() - Usuario ID {id} marcado como inactivo exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.Delete() - Error al hacer soft delete del usuario ID: {id}");
                throw;
            }
        }

        // Método adicional para eliminación física (solo para casos específicos)
        public async Task<bool> HardDelete(int id)
        {
            try
            {
                _logger.LogInformation($"UsuarioRepository.HardDelete() - Verificando posibilidad de hard delete para usuario ID: {id}");

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning($"UsuarioRepository.HardDelete() - Usuario ID {id} no encontrado");
                    return false;
                }

                // Verificar si tiene registros relacionados
                var tieneConfigSla = await _context.ConfigSlas
                    .AnyAsync(c => c.CreadoPor == id || c.ActualizadoPor == id);

                var tieneSolicitudes = await _context.Solicitudes
                    .AnyAsync(s => s.CreadoPor == id || s.ActualizadoPor == id);

                var tieneReportes = await _context.Reportes
                    .AnyAsync(r => r.GeneradoPor == id);

                if (tieneConfigSla || tieneSolicitudes || tieneReportes)
                {
                    _logger.LogWarning($"UsuarioRepository.HardDelete() - Usuario ID {id} tiene registros relacionados, no se puede eliminar");
                    return false;
                }

                // Solo eliminar si no tiene relaciones
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"UsuarioRepository.HardDelete() - Usuario ID {id} eliminado físicamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.HardDelete() - Error al hacer hard delete del usuario ID: {id}");
                throw;
            }
        }

        public async Task<bool> Exists(int id)
        {
            try
            {
                var existe = await _context.Usuarios.AnyAsync(u => u.IdUsuario == id);
                _logger.LogInformation($"UsuarioRepository.Exists({id}) - Usuario existe: {existe}");
                return existe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.Exists({id}) - Error al verificar existencia");
                throw;
            }
        }

        public async Task<bool> ExistsByUsername(string username)
        {
            try
            {
                var existe = await _context.Usuarios.AnyAsync(u => u.Username == username);
                _logger.LogInformation($"UsuarioRepository.ExistsByUsername('{username}') - Username existe: {existe}");
                return existe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.ExistsByUsername('{username}') - Error al verificar username");
                throw;
            }
        }

        public async Task<bool> ExistsByCorreo(string correo)
        {
            try
            {
                var existe = await _context.Usuarios.AnyAsync(u => u.Correo == correo);
                _logger.LogInformation($"UsuarioRepository.ExistsByCorreo('{correo}') - Correo existe: {existe}");
                return existe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UsuarioRepository.ExistsByCorreo('{correo}') - Error al verificar correo");
                throw;
            }
        }
    }
}