using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class RolesSistemaRepository : IRolesSistemaRepository
    {
        private readonly Proyecto01DbContext _context;

        public RolesSistemaRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolesSistemaListDTO>> GetAll()
        {
            return await _context.RolesSistema
                .Select(r => new RolesSistemaListDTO
                {
                    IdRolSistema = r.IdRolSistema,
                    Codigo = r.Codigo,
                    Nombre = r.Nombre,
                    EsActivo = r.EsActivo
                })
                .ToListAsync();
        }

        public async Task<RolesSistemaResponseDTO?> GetById(int id)
        {
            return await _context.RolesSistema
                .Where(r => r.IdRolSistema == id)
                .Select(r => new RolesSistemaResponseDTO
                {
                    IdRolSistema = r.IdRolSistema,
                    Codigo = r.Codigo,
                    Descripcion = r.Descripcion,
                    EsActivo = r.EsActivo,
                    Nombre = r.Nombre
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(RolesSistemaCreateDTO dto)
        {
            var rolSistema = new RolesSistema
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                EsActivo = dto.EsActivo,
                Nombre = dto.Nombre
            };

            _context.RolesSistema.Add(rolSistema);
            await _context.SaveChangesAsync();
            return rolSistema.IdRolSistema;
        }

        public async Task<int> Update(RolesSistemaUpdateDTO dto)
        {
            var rolSistema = await _context.RolesSistema.FindAsync(dto.IdRolSistema);
            if (rolSistema == null) return 0;

            if (dto.Codigo != null)
                rolSistema.Codigo = dto.Codigo;

            if (dto.Descripcion != null)
                rolSistema.Descripcion = dto.Descripcion;

            if (dto.EsActivo.HasValue)
                rolSistema.EsActivo = dto.EsActivo.Value;

            if (dto.Nombre != null)
                rolSistema.Nombre = dto.Nombre;

            _context.RolesSistema.Update(rolSistema);
            await _context.SaveChangesAsync();
            return rolSistema.IdRolSistema;
        }

        public async Task<bool> Delete(int id)
        {
            var rolSistema = await _context.RolesSistema.FindAsync(id);
            if (rolSistema == null) return false;

            _context.RolesSistema.Remove(rolSistema);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.RolesSistema.AnyAsync(r => r.IdRolSistema == id);
        }
    }
}
