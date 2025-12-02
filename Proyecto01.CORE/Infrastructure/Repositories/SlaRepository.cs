using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class SlaRepository : ISlaRepository
    {
        private readonly Proyecto01DbContext _context;

        public SlaRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Solicitud>> ObtenerSolicitudesPorSla(
            string tipoSla,
            int? mes = null,
            int? anio = null,
            int? idArea = null)
        {
            // Usar AsNoTracking para evitar problemas con propiedades de navegación ignoradas
            var tipoSlaUpper = tipoSla.ToUpper();
            
            // Query sin Include para evitar cargar relaciones problemáticas
            var query = from s in _context.Solicitudes.AsNoTracking()
                        join c in _context.ConfigSlas.AsNoTracking() on s.IdSla equals c.IdSla
                        where c.CodigoSla.ToUpper() == tipoSlaUpper
                        select s;

            // Filtrar por mes y año si se proporcionan
            if (mes.HasValue && anio.HasValue)
            {
                query = query.Where(s => 
                    s.FechaSolicitud.HasValue && 
                    s.FechaSolicitud.Value.Month == mes.Value && 
                    s.FechaSolicitud.Value.Year == anio.Value);
            }
            else if (anio.HasValue)
            {
                query = query.Where(s => 
                    s.FechaSolicitud.HasValue && 
                    s.FechaSolicitud.Value.Year == anio.Value);
            }

            // Filtrar por área si se proporciona
            if (idArea.HasValue)
            {
                query = query.Where(s => s.IdArea == idArea.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> ObtenerDiasUmbralSla(string tipoSla)
        {
            var tipoSlaUpper = tipoSla.ToUpper();
            
            // Proyectar solo la columna que necesitamos para evitar problemas con propiedades ignoradas
            var diasUmbral = await _context.ConfigSlas
                .AsNoTracking()
                .Where(c => c.CodigoSla.ToUpper() == tipoSlaUpper && c.EsActivo)
                .Select(c => c.DiasUmbral)
                .FirstOrDefaultAsync();

            if (diasUmbral == null)
            {
                throw new InvalidOperationException($"No se encontró configuración SLA para: {tipoSla}");
            }

            return diasUmbral.Value;
        }
    }
}
