using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.CORE.Infrastructure.Repositories
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly Proyecto01DbContext _context;

        public ReporteRepository(Proyecto01DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReporteListDTO>> GetAll()
        {
            return await _context.Reportes
                .Select(r => new ReporteListDTO
                {
                    IdReporte = r.IdReporte,
                    TipoReporte = r.TipoReporte,
                    Formato = r.Formato,
                    FechaGeneracion = r.FechaGeneracion
                })
                .ToListAsync();
        }

        public async Task<ReporteResponseDTO?> GetById(int id)
        {
            return await _context.Reportes
                .Where(r => r.IdReporte == id)
                .Select(r => new ReporteResponseDTO
                {
                    IdReporte = r.IdReporte,
                    TipoReporte = r.TipoReporte,
                    Formato = r.Formato,
                    FiltrosJson = r.FiltrosJson,
                    GeneradoPor = r.GeneradoPor,
                    FechaGeneracion = r.FechaGeneracion,
                    RutaArchivo = r.RutaArchivo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> Insert(ReporteCreateDTO dto)
        {
            var reporte = new Reporte
            {
                TipoReporte = dto.TipoReporte,
                Formato = dto.Formato,
                FiltrosJson = dto.FiltrosJson,
                GeneradoPor = dto.GeneradoPor,
                FechaGeneracion = DateTime.UtcNow,
                RutaArchivo = dto.RutaArchivo
            };

            _context.Reportes.Add(reporte);
            await _context.SaveChangesAsync();
            return reporte.IdReporte;
        }

        public async Task<int> Update(ReporteUpdateDTO dto)
        {
            var reporte = await _context.Reportes.FindAsync(dto.IdReporte);
            if (reporte == null) return 0;

            if (dto.RutaArchivo != null)
                reporte.RutaArchivo = dto.RutaArchivo;

            _context.Reportes.Update(reporte);
            await _context.SaveChangesAsync();
            return reporte.IdReporte;
        }

        public async Task<bool> Delete(int id)
        {
            var reporte = await _context.Reportes.FindAsync(id);
            if (reporte == null) return false;

            _context.Reportes.Remove(reporte);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Reportes.AnyAsync(r => r.IdReporte == id);
        }
    }
}
