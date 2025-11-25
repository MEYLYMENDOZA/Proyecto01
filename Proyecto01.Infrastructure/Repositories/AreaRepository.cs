using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.Infrastructure.Data;
using Proyecto01.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Proyecto01.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly Proyecto01DbContext _context;

        public AreaRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AreaListDTO>> GetAll()
        {
            return await _context.Areas
                .Select(a => new AreaListDTO
                {
                    IdArea = a.IdArea,
                    NombreArea = a.NombreArea
                })
                .ToListAsync();
        }

        public async Task<AreaListDTO?> GetById(int id)
        {
            return await _context.Areas
                .Where(a => a.IdArea == id)
                .Select(a => new AreaListDTO
                {
                    IdArea = a.IdArea,
                    NombreArea = a.NombreArea
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(AreaCreateDTO dto)
        {
            var area = new Area
            {
                NombreArea = dto.NombreArea,
                Descripcion = dto.Descripcion
            };

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return area.IdArea;
        }

        public async Task<int> Update(AreaUpdateDTO dto)
        {
            var area = await _context.Areas.FindAsync(dto.IdArea);
            if (area == null) return 0;

            area.NombreArea = dto.NombreArea;
            area.Descripcion = dto.Descripcion;

            _context.Areas.Update(area);
            await _context.SaveChangesAsync();
            return area.IdArea;
        }

        public async Task<bool> Delete(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return false;

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Areas.AnyAsync(a => a.IdArea == id);
        }
    }
}
