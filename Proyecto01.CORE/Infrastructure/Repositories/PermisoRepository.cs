using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class PermisoRepository : IPermisoRepository
    {
        private readonly Proyecto01DbContext _context;

        public PermisoRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermisoListDTO>> GetAll()
        {
            return await _context.Permisos
                .Select(p => new PermisoListDTO
                {
                    IdPermiso = p.IdPermiso,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre
                })
                .ToListAsync();
        }

        public async Task<PermisoResponseDTO?> GetById(int id)
        {
            return await _context.Permisos
                .Where(p => p.IdPermiso == id)
                .Select(p => new PermisoResponseDTO
                {
                    IdPermiso = p.IdPermiso,
                    Codigo = p.Codigo,
                    Descripcion = p.Descripcion,
                    Nombre = p.Nombre
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(PermisoCreateDTO dto)
        {
            var permiso = new Permiso
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                Nombre = dto.Nombre
            };

            _context.Permisos.Add(permiso);
            await _context.SaveChangesAsync();
            return permiso.IdPermiso;
        }

        public async Task<int> Update(PermisoUpdateDTO dto)
        {
            var permiso = await _context.Permisos.FindAsync(dto.IdPermiso);
            if (permiso == null) return 0;

            if (dto.Codigo != null)
                permiso.Codigo = dto.Codigo;

            if (dto.Descripcion != null)
                permiso.Descripcion = dto.Descripcion;

            if (dto.Nombre != null)
                permiso.Nombre = dto.Nombre;

            _context.Permisos.Update(permiso);
            await _context.SaveChangesAsync();
            return permiso.IdPermiso;
        }

        public async Task<bool> Delete(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null) return false;

            _context.Permisos.Remove(permiso);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Permisos.AnyAsync(p => p.IdPermiso == id);
        }
    }
}
