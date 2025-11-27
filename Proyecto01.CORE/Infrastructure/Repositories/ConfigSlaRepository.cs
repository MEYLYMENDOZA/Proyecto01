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

        public async Task<bool> Update(ConfigSlaUpdateDTO dto)
        {
            // 1. Buscar la entidad en la BD usando el CÓDIGO, no un ID.
            var configEntity = await _context.ConfigSlas
                                .FirstOrDefaultAsync(c => c.CodigoSla == dto.CodigoSla);

            // 2. Si no se encuentra, no se puede actualizar.
            if (configEntity == null)
            {
                return false;
            }

            // 3. Actualizar el valor en la entidad que encontraste.
            if (dto.DiasUmbral.HasValue)
            {
                configEntity.DiasUmbral = dto.DiasUmbral.Value;
            }
            
            // (Opcional) Actualizar otros campos si es necesario
            configEntity.ActualizadoEn = DateTime.UtcNow;
            if (dto.ActualizadoPor.HasValue)
            {
                configEntity.ActualizadoPor = dto.ActualizadoPor;
            }

            // 4. Guardar los cambios en la base de datos.
            var changes = await _context.SaveChangesAsync();

            // 5. Devolver 'true' solo si se guardó al menos un cambio.
            return changes > 0;
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
