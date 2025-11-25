using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly Proyecto01DbContext _context;

        public AlertaRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AlertaListDTO>> GetAll()
        {
            return await _context.Alertas
                .Select(a => new AlertaListDTO
                {
                    IdAlerta = a.IdAlerta,
                    IdSolicitud = a.IdSolicitud,
                    Nivel = a.Nivel,
                    Mensaje = a.Mensaje,
                    FechaCreacion = a.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<AlertaResponseDTO?> GetById(int id)
        {
            return await _context.Alertas
                .Where(a => a.IdAlerta == id)
                .Select(a => new AlertaResponseDTO
                {
                    IdAlerta = a.IdAlerta,
                    IdSolicitud = a.IdSolicitud,
                    IdTipoAlerta = a.IdTipoAlerta,
                    IdEstadoAlerta = a.IdEstadoAlerta,
                    Nivel = a.Nivel,
                    Mensaje = a.Mensaje,
                    EnviadoEmail = a.EnviadoEmail,
                    FechaCreacion = a.FechaCreacion,
                    FechaLectura = a.FechaLectura,
                    ActualizadoEn = a.ActualizadoEn
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(AlertaCreateDTO dto)
        {
            var alerta = new Alerta
            {
                IdSolicitud = dto.IdSolicitud,
                IdTipoAlerta = dto.IdTipoAlerta,
                IdEstadoAlerta = dto.IdEstadoAlerta,
                Nivel = dto.Nivel,
                Mensaje = dto.Mensaje,
                EnviadoEmail = dto.EnviadoEmail,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();
            return alerta.IdAlerta;
        }

        public async Task<int> Update(AlertaUpdateDTO dto)
        {
            var alerta = await _context.Alertas.FindAsync(dto.IdAlerta);
            if (alerta == null) return 0;

            if (dto.IdEstadoAlerta.HasValue)
                alerta.IdEstadoAlerta = dto.IdEstadoAlerta.Value;

            if (dto.Nivel != null)
                alerta.Nivel = dto.Nivel;

            if (dto.Mensaje != null)
                alerta.Mensaje = dto.Mensaje;

            if (dto.EnviadoEmail.HasValue)
                alerta.EnviadoEmail = dto.EnviadoEmail;

            if (dto.FechaLectura.HasValue)
                alerta.FechaLectura = dto.FechaLectura;

            alerta.ActualizadoEn = DateTime.UtcNow;

            _context.Alertas.Update(alerta);
            await _context.SaveChangesAsync();
            return alerta.IdAlerta;
        }

        public async Task<bool> Delete(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta == null) return false;

            _context.Alertas.Remove(alerta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Alertas.AnyAsync(a => a.IdAlerta == id);
        }
    }
}
