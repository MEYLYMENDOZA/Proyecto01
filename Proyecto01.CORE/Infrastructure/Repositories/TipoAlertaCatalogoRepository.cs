using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class TipoAlertaCatalogoRepository : ITipoAlertaCatalogoRepository
    {
        private readonly Proyecto01DbContext _context;

        public TipoAlertaCatalogoRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoAlertaCatalogoListDTO>> GetAll()
        {
            return await _context.TiposAlerta
                .Select(t => new TipoAlertaCatalogoListDTO
                {
                    IdTipoAlerta = t.IdTipoAlerta,
                    Codigo = t.Codigo,
                    Descripcion = t.Descripcion
                })
                .ToListAsync();
        }

        public async Task<TipoAlertaCatalogoResponseDTO?> GetById(int id)
        {
            return await _context.TiposAlerta
                .Where(t => t.IdTipoAlerta == id)
                .Select(t => new TipoAlertaCatalogoResponseDTO
                {
                    IdTipoAlerta = t.IdTipoAlerta,
                    Codigo = t.Codigo,
                    Descripcion = t.Descripcion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(TipoAlertaCatalogoCreateDTO dto)
        {
            var tipo = new TipoAlertaCatalogo
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion
            };

            _context.TiposAlerta.Add(tipo);
            await _context.SaveChangesAsync();
            return tipo.IdTipoAlerta;
        }

        public async Task<int> Update(TipoAlertaCatalogoUpdateDTO dto)
        {
            var tipo = await _context.TiposAlerta.FindAsync(dto.IdTipoAlerta);
            if (tipo == null) return 0;

            if (dto.Codigo != null)
                tipo.Codigo = dto.Codigo;

            if (dto.Descripcion != null)
                tipo.Descripcion = dto.Descripcion;

            _context.TiposAlerta.Update(tipo);
            await _context.SaveChangesAsync();
            return tipo.IdTipoAlerta;
        }

        public async Task<bool> Delete(int id)
        {
            var tipo = await _context.TiposAlerta.FindAsync(id);
            if (tipo == null) return false;

            _context.TiposAlerta.Remove(tipo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.TiposAlerta.AnyAsync(t => t.IdTipoAlerta == id);
        }
    }
}
