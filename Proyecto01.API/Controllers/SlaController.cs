using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Infrastructure.Data;

namespace Proyecto01.API.Controllers
{
    /// <summary>
    /// Controlador para endpoints de predicción SLA consumidos por la app Android
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SlaController : ControllerBase
    {
        private readonly Proyecto01DbContext _context;

        public SlaController(Proyecto01DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene solicitudes con datos SLA para cálculo de predicción
        /// GET /api/sla/solicitudes
        /// </summary>
        /// <param name="meses">Número de meses hacia atrás desde hoy (por defecto 12)</param>
        /// <param name="anio">Año específico (opcional, si se usa ignora 'meses')</param>
        /// <param name="mes">Mes específico 1-12 (opcional, requiere 'anio')</param>
        /// <param name="idArea">Filtrar por área específica (opcional)</param>
        /// <returns>Lista de solicitudes con datos para calcular cumplimiento SLA</returns>
        [HttpGet("solicitudes")]
        public async Task<IActionResult> GetSolicitudes(
            [FromQuery] int? meses = 12,
            [FromQuery] int? anio = null,
            [FromQuery] int? mes = null,
            [FromQuery] int? idArea = null)
        {
            try
            {
                // Iniciar query base con includes necesarios
                var query = _context.Solicitudes
                    .Include(s => s.ConfigSla)
                    .AsQueryable();

                // Aplicar filtros de fecha
                if (anio.HasValue && mes.HasValue)
                {
                    // Filtro por mes/año específico
                    query = query.Where(s => s.FechaSolicitud.HasValue &&
                                             s.FechaSolicitud.Value.Year == anio.Value &&
                                             s.FechaSolicitud.Value.Month == mes.Value);
                }
                else if (anio.HasValue)
                {
                    // Solo año (todos los meses de ese año)
                    query = query.Where(s => s.FechaSolicitud.HasValue &&
                                             s.FechaSolicitud.Value.Year == anio.Value);
                }
                else if (meses.HasValue)
                {
                    // Últimos N meses desde hoy
                    var fechaInicio = DateTime.UtcNow.AddMonths(-meses.Value);
                    query = query.Where(s => s.FechaSolicitud.HasValue &&
                                             s.FechaSolicitud.Value >= fechaInicio);
                }

                // Filtrar por área si se especifica
                if (idArea.HasValue)
                {
                    query = query.Where(s => s.IdArea == idArea.Value);
                }

                // Proyectar a DTO que Android espera
                var solicitudes = await query
                    .OrderBy(s => s.FechaSolicitud)
                    .Select(s => new
                    {
                        idSolicitud = s.IdSolicitud,
                        // Formato ISO para Android (yyyy-MM-ddTHH:mm:ss)
                        fechaSolicitud = s.FechaSolicitud.HasValue
                            ? s.FechaSolicitud.Value.ToString("yyyy-MM-ddTHH:mm:ss")
                            : DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                        // Días que tomó resolver la solicitud
                        numDiasSla = s.NumDiasSla ??
                            (s.FechaSolicitud.HasValue && s.FechaIngreso.HasValue
                                ? (int)(s.FechaIngreso.Value - s.FechaSolicitud.Value).TotalDays
                                : 0),
                        // Días máximos permitidos según configuración SLA
                        diasUmbral = s.ConfigSla != null ? (s.ConfigSla.DiasUmbral ?? 30) : 30,
                        idArea = s.IdArea,
                        codigoSla = s.ConfigSla != null ? s.ConfigSla.CodigoSla : "N/A"
                    })
                    .ToListAsync();

                // Log para debugging
                Console.WriteLine($"[SlaController] Solicitudes encontradas: {solicitudes.Count}");
                Console.WriteLine($"[SlaController] Filtros: meses={meses}, anio={anio}, mes={mes}, idArea={idArea}");

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SlaController] ERROR: {ex.Message}");
                return StatusCode(500, new
                {
                    error = "Error al obtener solicitudes",
                    detail = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }


        /// <summary>
        /// Endpoint de prueba para verificar conectividad
        /// GET /api/sla/ping
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                status = "online",
                message = "SLA Controller funcionando correctamente",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                version = "1.0"
            });
        }

        /// <summary>
        /// Obtiene estadísticas básicas de SLA (opcional, para debugging)
        /// GET /api/sla/estadisticas
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<IActionResult> GetEstadisticas([FromQuery] int meses = 12)
        {
            try
            {
                var fechaInicio = DateTime.UtcNow.AddMonths(-meses);

                var total = await _context.Solicitudes
                    .Where(s => s.FechaSolicitud.HasValue && s.FechaSolicitud.Value >= fechaInicio)
                    .CountAsync();

                var cumplidas = await _context.Solicitudes
                    .Include(s => s.ConfigSla)
                    .Where(s => s.FechaSolicitud.HasValue &&
                               s.FechaSolicitud.Value >= fechaInicio &&
                               s.NumDiasSla.HasValue &&
                               s.ConfigSla != null &&
                               s.NumDiasSla.Value <= s.ConfigSla.DiasUmbral)
                    .CountAsync();

                var porcentaje = total > 0 ? (cumplidas * 100.0 / total) : 0;

                return Ok(new
                {
                    periodoMeses = meses,
                    totalSolicitudes = total,
                    solicitudesCumplidas = cumplidas,
                    solicitudesIncumplidas = total - cumplidas,
                    porcentajeCumplimiento = Math.Round(porcentaje, 2)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

