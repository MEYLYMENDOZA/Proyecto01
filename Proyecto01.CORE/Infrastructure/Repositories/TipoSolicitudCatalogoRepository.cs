using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class TipoSolicitudCatalogoRepository : ITipoSolicitudCatalogoRepository
    {
        private readonly Proyecto01DbContext _context;

        public TipoSolicitudCatalogoRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoSolicitudCatalogoListDTO>> GetAll()
        {
            return await _context.TiposSolicitud
                .Select(t => new TipoSolicitudCatalogoListDTO
                {
                    IdTipoSolicitud = t.IdTipoSolicitud,
                    Codigo = t.Codigo,
                    Descripcion = t.Descripcion
                })
                .ToListAsync();
        }

        public async Task<TipoSolicitudCatalogoResponseDTO?> GetById(int id)
        {
            return await _context.TiposSolicitud
                .Where(t => t.IdTipoSolicitud == id)
                .Select(t => new TipoSolicitudCatalogoResponseDTO
                {
                    IdTipoSolicitud = t.IdTipoSolicitud,
                    Codigo = t.Codigo,
                    Descripcion = t.Descripcion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(TipoSolicitudCatalogoCreateDTO dto)
        {
            var tipo = new TipoSolicitudCatalogo
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion
            };

            _context.TiposSolicitud.Add(tipo);
            await _context.SaveChangesAsync();
            return tipo.IdTipoSolicitud;
        }

        public async Task<int> Update(TipoSolicitudCatalogoUpdateDTO dto)
        {
            var tipo = await _context.TiposSolicitud.FindAsync(dto.IdTipoSolicitud);
            if (tipo == null) return 0;

            if (dto.Codigo != null)
                tipo.Codigo = dto.Codigo;

            if (dto.Descripcion != null)
                tipo.Descripcion = dto.Descripcion;

            _context.TiposSolicitud.Update(tipo);
            await _context.SaveChangesAsync();
            return tipo.IdTipoSolicitud;
        }

        public async Task<bool> Delete(int id)
        {
            var tipo = await _context.TiposSolicitud.FindAsync(id);
            if (tipo == null) return false;

            _context.TiposSolicitud.Remove(tipo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.TiposSolicitud.AnyAsync(t => t.IdTipoSolicitud == id);
        }
    }
}
