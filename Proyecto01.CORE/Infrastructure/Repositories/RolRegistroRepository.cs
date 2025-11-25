using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class RolRegistroRepository : IRolRegistroRepository
    {
        private readonly Proyecto01DbContext _context;

        public RolRegistroRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolRegistroListDTO>> GetAll()
        {
            return await _context.RolesRegistro
                .Select(r => new RolRegistroListDTO
                {
                    IdRolRegistro = r.IdRolRegistro,
                    NombreRol = r.NombreRol,
                    EsActivo = r.EsActivo
                })
                .ToListAsync();
        }

        public async Task<RolRegistroResponseDTO?> GetById(int id)
        {
            return await _context.RolesRegistro
                .Where(r => r.IdRolRegistro == id)
                .Select(r => new RolRegistroResponseDTO
                {
                    IdRolRegistro = r.IdRolRegistro,
                    BloqueTeach = r.BloqueTeach,
                    Descripcion = r.Descripcion,
                    EsActivo = r.EsActivo,
                    NombreRol = r.NombreRol
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(RolRegistroCreateDTO dto)
        {
            var rolRegistro = new RolRegistro
            {
                BloqueTeach = dto.BloqueTeach,
                Descripcion = dto.Descripcion,
                EsActivo = dto.EsActivo,
                NombreRol = dto.NombreRol
            };

            _context.RolesRegistro.Add(rolRegistro);
            await _context.SaveChangesAsync();
            return rolRegistro.IdRolRegistro;
        }

        public async Task<int> Update(RolRegistroUpdateDTO dto)
        {
            var rolRegistro = await _context.RolesRegistro.FindAsync(dto.IdRolRegistro);
            if (rolRegistro == null) return 0;

            if (dto.BloqueTeach != null)
                rolRegistro.BloqueTeach = dto.BloqueTeach;

            if (dto.Descripcion != null)
                rolRegistro.Descripcion = dto.Descripcion;

            if (dto.EsActivo.HasValue)
                rolRegistro.EsActivo = dto.EsActivo.Value;

            if (dto.NombreRol != null)
                rolRegistro.NombreRol = dto.NombreRol;

            _context.RolesRegistro.Update(rolRegistro);
            await _context.SaveChangesAsync();
            return rolRegistro.IdRolRegistro;
        }

        public async Task<bool> Delete(int id)
        {
            var rolRegistro = await _context.RolesRegistro.FindAsync(id);
            if (rolRegistro == null) return false;

            _context.RolesRegistro.Remove(rolRegistro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.RolesRegistro.AnyAsync(r => r.IdRolRegistro == id);
        }
    }
}
