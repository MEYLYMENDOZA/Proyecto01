using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class PersonalRepository : IPersonalRepository
    {
        private readonly Proyecto01DbContext _context;

        public PersonalRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonalListDTO>> GetAll()
        {
            return await _context.Personales
                .Select(p => new PersonalListDTO
                {
                    IdPersonal = p.IdPersonal,
                    IdUsuario = p.IdUsuario,
                    Nombres = p.Nombres,
                    Apellidos = p.Apellidos,
                    Documento = p.Documento,
                    Estado = p.Estado
                })
                .ToListAsync();
        }

        public async Task<PersonalListDTO?> GetById(int id)
        {
            return await _context.Personales
                .Where(p => p.IdPersonal == id)
                .Select(p => new PersonalListDTO
                {
                    IdPersonal = p.IdPersonal,
                    IdUsuario = p.IdUsuario,
                    Nombres = p.Nombres,
                    Apellidos = p.Apellidos,
                    Documento = p.Documento,
                    Estado = p.Estado
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PersonalListDTO?> GetByIdUsuario(int idUsuario)
        {
            return await _context.Personales
                .Where(p => p.IdUsuario == idUsuario)
                .Select(p => new PersonalListDTO
                {
                    IdPersonal = p.IdPersonal,
                    IdUsuario = p.IdUsuario,
                    Nombres = p.Nombres,
                    Apellidos = p.Apellidos,
                    Documento = p.Documento,
                    Estado = p.Estado
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(PersonalCreateDTO dto)
        {
            var personal = new Personal
            {
                IdUsuario = dto.IdUsuario,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Documento = dto.Documento,
                CreadoEn = DateTime.UtcNow
            };

            _context.Personales.Add(personal);
            await _context.SaveChangesAsync();
            return personal.IdPersonal;
        }

        public async Task<int> Update(PersonalUpdateDTO dto)
        {
            var personal = await _context.Personales.FindAsync(dto.IdPersonal);
            if (personal == null) return 0;

            if (!string.IsNullOrEmpty(dto.Nombres))
                personal.Nombres = dto.Nombres;

            if (!string.IsNullOrEmpty(dto.Apellidos))
                personal.Apellidos = dto.Apellidos;

            if (!string.IsNullOrEmpty(dto.Documento))
                personal.Documento = dto.Documento;

            if (!string.IsNullOrEmpty(dto.Estado))
                personal.Estado = dto.Estado;

            personal.ActualizadoEn = DateTime.UtcNow;

            _context.Personales.Update(personal);
            await _context.SaveChangesAsync();
            return personal.IdPersonal;
        }

        public async Task<bool> Delete(int id)
        {
            var personal = await _context.Personales.FindAsync(id);
            if (personal == null) return false;

            _context.Personales.Remove(personal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Personales.AnyAsync(p => p.IdPersonal == id);
        }
    }
}
