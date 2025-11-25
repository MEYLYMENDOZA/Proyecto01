using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class EstadoSolicitudCatalogoRepository : IEstadoSolicitudCatalogoRepository
    {
        private readonly Proyecto01DbContext _context;

        public EstadoSolicitudCatalogoRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EstadoSolicitudCatalogoListDTO>> GetAll()
        {
            return await _context.EstadosSolicitud
                .Select(e => new EstadoSolicitudCatalogoListDTO
                {
                    IdEstadoSolicitud = e.IdEstadoSolicitud,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion
                })
                .ToListAsync();
        }

        public async Task<EstadoSolicitudCatalogoResponseDTO?> GetById(int id)
        {
            return await _context.EstadosSolicitud
                .Where(e => e.IdEstadoSolicitud == id)
                .Select(e => new EstadoSolicitudCatalogoResponseDTO
                {
                    IdEstadoSolicitud = e.IdEstadoSolicitud,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(EstadoSolicitudCatalogoCreateDTO dto)
        {
            var estado = new EstadoSolicitudCatalogo
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion
            };

            _context.EstadosSolicitud.Add(estado);
            await _context.SaveChangesAsync();
            return estado.IdEstadoSolicitud;
        }

        public async Task<int> Update(EstadoSolicitudCatalogoUpdateDTO dto)
        {
            var estado = await _context.EstadosSolicitud.FindAsync(dto.IdEstadoSolicitud);
            if (estado == null) return 0;

            if (dto.Codigo != null)
                estado.Codigo = dto.Codigo;

            if (dto.Descripcion != null)
                estado.Descripcion = dto.Descripcion;

            _context.EstadosSolicitud.Update(estado);
            await _context.SaveChangesAsync();
            return estado.IdEstadoSolicitud;
        }

        public async Task<bool> Delete(int id)
        {
            var estado = await _context.EstadosSolicitud.FindAsync(id);
            if (estado == null) return false;

            _context.EstadosSolicitud.Remove(estado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.EstadosSolicitud.AnyAsync(e => e.IdEstadoSolicitud == id);
        }
    }
}
