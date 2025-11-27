using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class TendenciaLogRepository : ITendenciaLogRepository
    {
        private readonly Proyecto01DbContext _context;

        public TendenciaLogRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<int> GuardarLog(PrediccionTendenciaLog log)
        {
            _context.PrediccionTendenciaLogs.Add(log);
            await _context.SaveChangesAsync();
            return log.IdLog;
        }

        public async Task<PrediccionTendenciaLog?> ObtenerUltimoLog(string tipoSla, int? idArea = null)
        {
            var query = _context.PrediccionTendenciaLogs
                .Where(l => l.TipoSla == tipoSla);

            if (idArea.HasValue)
            {
                query = query.Where(l => l.IdArea == idArea.Value);
            }

            return await query
                .OrderByDescending(l => l.FechaAnalisis)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PrediccionTendenciaLog>> ObtenerHistorial(
            string tipoSla,
            int? idArea = null,
            int limite = 12)
        {
            var query = _context.PrediccionTendenciaLogs
                .Where(l => l.TipoSla == tipoSla);

            if (idArea.HasValue)
            {
                query = query.Where(l => l.IdArea == idArea.Value);
            }

            return await query
                .OrderByDescending(l => l.FechaAnalisis)
                .Take(limite)
                .ToListAsync();
        }
    }
}
