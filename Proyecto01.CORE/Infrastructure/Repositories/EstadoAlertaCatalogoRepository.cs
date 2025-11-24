using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class EstadoAlertaCatalogoRepository : IEstadoAlertaCatalogoRepository
    {
        private readonly Proyecto01DbContext _context;

        public EstadoAlertaCatalogoRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EstadoAlertaCatalogoListDTO>> GetAll()
        {
            return await _context.EstadosAlerta
                .Select(e => new EstadoAlertaCatalogoListDTO
                {
                    IdEstadoAlerta = e.IdEstadoAlerta,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion
                })
                .ToListAsync();
        }

        public async Task<EstadoAlertaCatalogoResponseDTO?> GetById(int id)
        {
            return await _context.EstadosAlerta
                .Where(e => e.IdEstadoAlerta == id)
                .Select(e => new EstadoAlertaCatalogoResponseDTO
                {
                    IdEstadoAlerta = e.IdEstadoAlerta,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(EstadoAlertaCatalogoCreateDTO dto)
        {
            var estado = new EstadoAlertaCatalogo
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion
            };

            _context.EstadosAlerta.Add(estado);
            await _context.SaveChangesAsync();
            return estado.IdEstadoAlerta;
        }

        public async Task<int> Update(EstadoAlertaCatalogoUpdateDTO dto)
        {
            var estado = await _context.EstadosAlerta.FindAsync(dto.IdEstadoAlerta);
            if (estado == null) return 0;

            if (dto.Codigo != null)
                estado.Codigo = dto.Codigo;

            if (dto.Descripcion != null)
                estado.Descripcion = dto.Descripcion;

            _context.EstadosAlerta.Update(estado);
            await _context.SaveChangesAsync();
            return estado.IdEstadoAlerta;
        }

        public async Task<bool> Delete(int id)
        {
            var estado = await _context.EstadosAlerta.FindAsync(id);
            if (estado == null) return false;

            _context.EstadosAlerta.Remove(estado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.EstadosAlerta.AnyAsync(e => e.IdEstadoAlerta == id);
        }
    }
}
