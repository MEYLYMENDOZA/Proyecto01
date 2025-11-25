using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class ConfigSlaRepository : IConfigSlaRepository
    {
        private readonly Proyecto01DbContext _context;

        public ConfigSlaRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConfigSlaListDTO>> GetAll()
        {
            return await _context.ConfigSlas
                .Select(c => new ConfigSlaListDTO
                {
                    IdSla = c.IdSla,
                    CodigoSla = c.CodigoSla,
                    Descripcion = c.Descripcion,
                    DiasUmbral = c.DiasUmbral,
                    EsActivo = c.EsActivo
                })
                .ToListAsync();
        }

        public async Task<ConfigSlaResponseDTO?> GetById(int id)
        {
            return await _context.ConfigSlas
                .Where(c => c.IdSla == id)
                .Select(c => new ConfigSlaResponseDTO
                {
                    IdSla = c.IdSla,
                    CodigoSla = c.CodigoSla,
                    Descripcion = c.Descripcion,
                    DiasUmbral = c.DiasUmbral,
                    EsActivo = c.EsActivo,
                    IdTipoSolicitud = c.IdTipoSolicitud,
                    CreadoEn = c.CreadoEn,
                    ActualizadoEn = c.ActualizadoEn,
                    CreadoPor = c.CreadoPor,
                    ActualizadoPor = c.ActualizadoPor
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(ConfigSlaCreateDTO dto)
        {
            var configSla = new ConfigSla
            {
                CodigoSla = dto.CodigoSla,
                Descripcion = dto.Descripcion,
                DiasUmbral = dto.DiasUmbral,
                EsActivo = dto.EsActivo,
                IdTipoSolicitud = dto.IdTipoSolicitud,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = dto.CreadoPor
            };

            _context.ConfigSlas.Add(configSla);
            await _context.SaveChangesAsync();
            return configSla.IdSla;
        }

        public async Task<int> Update(ConfigSlaUpdateDTO dto)
        {
            var configSla = await _context.ConfigSlas.FindAsync(dto.IdSla);
            if (configSla == null) return 0;

            if (dto.CodigoSla != null)
                configSla.CodigoSla = dto.CodigoSla;

            if (dto.Descripcion != null)
                configSla.Descripcion = dto.Descripcion;

            if (dto.DiasUmbral.HasValue)
                configSla.DiasUmbral = dto.DiasUmbral;

            if (dto.EsActivo.HasValue)
                configSla.EsActivo = dto.EsActivo.Value;

            if (dto.IdTipoSolicitud.HasValue)
                configSla.IdTipoSolicitud = dto.IdTipoSolicitud.Value;

            configSla.ActualizadoEn = DateTime.UtcNow;
            configSla.ActualizadoPor = dto.ActualizadoPor;

            _context.ConfigSlas.Update(configSla);
            await _context.SaveChangesAsync();
            return configSla.IdSla;
        }

        public async Task<bool> Delete(int id)
        {
            var configSla = await _context.ConfigSlas.FindAsync(id);
            if (configSla == null) return false;

            _context.ConfigSlas.Remove(configSla);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.ConfigSlas.AnyAsync(c => c.IdSla == id);
        }
    }
}
