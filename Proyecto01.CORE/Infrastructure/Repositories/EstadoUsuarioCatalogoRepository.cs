using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class EstadoUsuarioCatalogoRepository : IEstadoUsuarioCatalogoRepository
    {
        private readonly Proyecto01DbContext _context;

        public EstadoUsuarioCatalogoRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EstadoUsuarioCatalogoListDTO>> GetAll()
        {
            return await _context.EstadosUsuario
                .Select(e => new EstadoUsuarioCatalogoListDTO
                {
                    IdEstadoUsuario = e.IdEstadoUsuario,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion
                })
                .ToListAsync();
        }

        public async Task<EstadoUsuarioCatalogoResponseDTO?> GetById(int id)
        {
            return await _context.EstadosUsuario
                .Where(e => e.IdEstadoUsuario == id)
                .Select(e => new EstadoUsuarioCatalogoResponseDTO
                {
                    IdEstadoUsuario = e.IdEstadoUsuario,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(EstadoUsuarioCatalogoCreateDTO dto)
        {
            var estado = new EstadoUsuarioCatalogo
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion
            };

            _context.EstadosUsuario.Add(estado);
            await _context.SaveChangesAsync();
            return estado.IdEstadoUsuario;
        }

        public async Task<int> Update(EstadoUsuarioCatalogoUpdateDTO dto)
        {
            var estado = await _context.EstadosUsuario.FindAsync(dto.IdEstadoUsuario);
            if (estado == null) return 0;

            if (dto.Codigo != null)
                estado.Codigo = dto.Codigo;

            if (dto.Descripcion != null)
                estado.Descripcion = dto.Descripcion;

            _context.EstadosUsuario.Update(estado);
            await _context.SaveChangesAsync();
            return estado.IdEstadoUsuario;
        }

        public async Task<bool> Delete(int id)
        {
            var estado = await _context.EstadosUsuario.FindAsync(id);
            if (estado == null) return false;

            _context.EstadosUsuario.Remove(estado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.EstadosUsuario.AnyAsync(e => e.IdEstadoUsuario == id);
        }
    }
}
