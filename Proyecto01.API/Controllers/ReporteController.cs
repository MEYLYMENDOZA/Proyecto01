using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Application.Services;
using System;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly TendenciaService _tendenciaService;
        private readonly ISolicitudRepository _solicitudRepository;
        private readonly IConfigSlaRepository _configSlaRepository;
        private readonly ISlaRepository _slaRepository;

        public ReporteController(
            IReporteService reporteService,
            TendenciaService tendenciaService,
            ISolicitudRepository solicitudRepository,
            IConfigSlaRepository configSlaRepository,
            ISlaRepository slaRepository)
        {
            _reporteService = reporteService;
            _tendenciaService = tendenciaService;
            _solicitudRepository = solicitudRepository;
            _configSlaRepository = configSlaRepository;
            _slaRepository = slaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reportes = await _reporteService.GetAll();
            return Ok(reportes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reporte = await _reporteService.GetById(id);
            if (reporte == null)
                return NotFound($"Reporte con ID {id} no encontrado.");

            return Ok(reporte);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReporteCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _reporteService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ReporteUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reporteService.Update(dto);
            if (result == 0)
                return NotFound($"Reporte con ID {dto.IdReporte} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reporteService.Delete(id);
            if (!result)
                return NotFound($"Reporte con ID {id} no encontrado.");

            return NoContent();
        }

        /// <summary>
        /// US-12: Obtiene el reporte de tendencia y proyección SLA
        /// GET /api/reporte/tendencia
        /// </summary>
        /// <param name="mes">Mes de análisis (1-12) - opcional</param>
        /// <param name="anio">Año de análisis - opcional</param>
        /// <param name="tipoSla">Tipo de SLA (ejemplo: SLA001, SLA002) - obligatorio</param>
        /// <param name="rol">ID del área/rol - opcional</param>
        [HttpGet("tendencia")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTendencia(
            [FromQuery] int? mes = null,
            [FromQuery] int? anio = null,
            [FromQuery] string? tipoSla = null,
            [FromQuery] int? rol = null)
        {
            try
            {
                // OWASP: Obtener IP del cliente para auditoría
                var ipCliente = HttpContext.Connection.RemoteIpAddress?.ToString();

                // OWASP: Validar usuario autenticado (descomentar cuando se implemente autenticación)
                // var usuario = User.Identity?.Name;
                // if (string.IsNullOrEmpty(usuario))
                // {
                //     return Unauthorized(new { error = "Usuario no autenticado" });
                // }

                // Usuario temporal para desarrollo
                var usuario = "admin";

                // Llamar al servicio de negocio
                var reporte = await _tendenciaService.GenerarReporteTendencia(
                    mes: mes,
                    anio: anio,
                    tipoSla: tipoSla,
                    idArea: rol,
                    usuarioSolicitante: usuario,
                    ipCliente: ipCliente
                );

                // Log para debugging
                Console.WriteLine($"[ReportesController] Reporte generado exitosamente");
                Console.WriteLine($"[ReportesController] Parámetros: mes={mes}, año={anio}, tipoSla={tipoSla}, rol={rol}");
                Console.WriteLine($"[ReportesController] Proyección: {reporte.Proyeccion}%, Tendencia: {reporte.EstadoTendencia}");

                // OWASP: No exponer información sensible
                return Ok(new
                {
                    historico = reporte.Historico,
                    tendencia = reporte.Tendencia,
                    proyeccion = reporte.Proyeccion,
                    pendiente = reporte.Pendiente,
                    intercepto = reporte.Intercepto,
                    estadoTendencia = reporte.EstadoTendencia,
                    // Metadatos adicionales (opcional)
                    metadata = new
                    {
                        totalRegistros = reporte.TotalRegistros,
                        fechaGeneracion = reporte.FechaGeneracion.ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                });
            }
            catch (ArgumentException ex)
            {
                // Error de validación de parámetros
                Console.WriteLine($"[ReportesController] Error de validación: {ex.Message}");
                return BadRequest(new
                {
                    error = "Parámetros inválidos",
                    detalle = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej: datos insuficientes)
                Console.WriteLine($"[ReportesController] Error de negocio: {ex.Message}");
                return BadRequest(new
                {
                    error = "No se puede generar el reporte",
                    detalle = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Error no controlado
                Console.WriteLine($"[ReportesController] Error interno: {ex.Message}");
                Console.WriteLine($"[ReportesController] Stack trace: {ex.StackTrace}");

                // OWASP: No exponer detalles internos en producción
                return StatusCode(500, new
                {
                    error = "Error interno del servidor",
                    detalle = "Ocurrió un error al procesar la solicitud. Contacte al administrador."
                });
            }
        }

        /// <summary>
        /// Obtiene los años disponibles en las solicitudes
        /// GET /api/reporte/anios-disponibles
        /// </summary>
        [HttpGet("anios-disponibles")]
        [ProducesResponseType(typeof(List<int>), 200)]
        public async Task<IActionResult> GetAniosDisponibles()
        {
            try
            {
                var solicitudes = await _solicitudRepository.GetAll();
                var anios = solicitudes
                    .Where(s => s.FechaSolicitud.HasValue)
                    .Select(s => s.FechaSolicitud!.Value.Year)
                    .Distinct()
                    .OrderByDescending(a => a)
                    .ToList();

                Console.WriteLine($"[ReportesController] Años disponibles: {string.Join(", ", anios)}");
                return Ok(anios);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportesController] Error al obtener años: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener años disponibles" });
            }
        }

        /// <summary>
        /// Obtiene las áreas disponibles
        /// GET /api/reporte/areas-disponibles
        /// </summary>
        [HttpGet("areas-disponibles")]
        [ProducesResponseType(typeof(List<object>), 200)]
        public async Task<IActionResult> GetAreasDisponibles()
        {
            try
            {
                var solicitudes = await _solicitudRepository.GetAll();
                var areas = solicitudes
                    .Select(s => new { id = s.IdArea, nombre = $"Área {s.IdArea}" })
                    .GroupBy(a => a.id)
                    .Select(g => g.First())
                    .OrderBy(a => a.id)
                    .ToList();

                Console.WriteLine($"[ReportesController] Áreas disponibles: {areas.Count}");
                return Ok(areas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportesController] Error al obtener áreas: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener áreas disponibles" });
            }
        }

        /// <summary>
        /// Obtiene los períodos (meses) disponibles para un año específico
        /// GET /api/reporte/periodos-disponibles?anio=2024
        /// </summary>
        [HttpGet("periodos-disponibles")]
        [ProducesResponseType(typeof(List<int>), 200)]
        public async Task<IActionResult> GetPeriodosDisponibles([FromQuery] int? anio = null)
        {
            try
            {
                var solicitudes = await _solicitudRepository.GetAll();
                
                var query = solicitudes.Where(s => s.FechaSolicitud.HasValue);
                
                if (anio.HasValue)
                {
                    query = query.Where(s => s.FechaSolicitud!.Value.Year == anio.Value);
                }

                var meses = query
                    .Select(s => s.FechaSolicitud!.Value.Month)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();

                return Ok(meses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportesController] Error al obtener períodos: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener períodos disponibles" });
            }
        }

        /// <summary>
        /// Obtiene los tipos de SLA disponibles
        /// GET /api/reporte/tipos-sla-disponibles
        /// </summary>
        [HttpGet("tipos-sla-disponibles")]
        [ProducesResponseType(typeof(List<object>), 200)]
        public async Task<IActionResult> GetTiposSlaDisponibles()
        {
            try
            {
                var configsSla = await _configSlaRepository.GetAll();
                var tiposSla = configsSla
                    .Where(c => c.EsActivo)
                    .Select(c => new 
                    { 
                        codigo = c.CodigoSla,
                        descripcion = c.Descripcion ?? c.CodigoSla,
                        diasUmbral = c.DiasUmbral ?? 0
                    })
                    .Distinct()
                    .ToList();

                return Ok(tiposSla);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportesController] Error al obtener tipos SLA: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener tipos SLA disponibles" });
            }
        }

        /// <summary>
        /// Obtiene los meses disponibles para un año específico (alias de periodos-disponibles)
        /// GET /api/reporte/meses-disponibles?anio=2024
        /// </summary>
        [HttpGet("meses-disponibles")]
        [ProducesResponseType(typeof(List<int>), 200)]
        public async Task<IActionResult> GetMesesDisponibles([FromQuery] int? anio = null)
        {
            try
            {
                var solicitudes = await _solicitudRepository.GetAll();
                
                var query = solicitudes.Where(s => s.FechaSolicitud.HasValue);
                
                if (anio.HasValue)
                {
                    query = query.Where(s => s.FechaSolicitud!.Value.Year == anio.Value);
                }

                var meses = query
                    .Select(s => s.FechaSolicitud!.Value.Month)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();

                Console.WriteLine($"[ReportesController] Meses disponibles para año {anio}: {string.Join(", ", meses)}");
                return Ok(meses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportesController] Error al obtener meses: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener meses disponibles" });
            }
        }

        /// <summary>
        /// US-12: Obtiene solicitudes para análisis de tendencia (datos crudos)
        /// La APP calcula la tendencia y proyección, NO el backend
        /// GET /api/reporte/solicitudes-tendencia
        /// </summary>
        /// <param name="anio">Año de análisis - opcional (últimos 12 meses si no se especifica)</param>
        /// <param name="tipoSla">Tipo de SLA - obligatorio (ejemplo: SLA001, SLA002)</param>
        /// <param name="idArea">ID del área - opcional</param>
        [HttpGet("solicitudes-tendencia")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSolicitudesTendencia(
            [FromQuery] int? anio = null,
            [FromQuery] string? tipoSla = null,
            [FromQuery] int? idArea = null)
        {
            try
            {
                Console.WriteLine($"[ReporteController] Solicitud de datos tendencia: año={anio}, tipoSla={tipoSla}, area={idArea}");

                // Validar parámetros básicos
                if (string.IsNullOrWhiteSpace(tipoSla))
                {
                    return BadRequest(new { error = "El parámetro tipoSla es obligatorio" });
                }

                // Sanitizar entrada
                tipoSla = tipoSla.Trim().ToUpperInvariant();

                // Calcular rango de fechas (últimos 12 meses del año especificado)
                DateTime fechaFin;
                DateTime fechaInicio;

                if (anio.HasValue)
                {
                    fechaFin = new DateTime(anio.Value, 12, 31, 23, 59, 59);
                    fechaInicio = fechaFin.AddMonths(-11).Date; // 12 meses hacia atrás
                }
                else
                {
                    fechaFin = DateTime.Now;
                    fechaInicio = fechaFin.AddMonths(-11).Date;
                }

                Console.WriteLine($"[ReporteController] Rango de fechas: {fechaInicio:yyyy-MM-dd} a {fechaFin:yyyy-MM-dd}");

                // Obtener configuración SLA usando el repositorio
                var diasUmbral = 0;
                try
                {
                    diasUmbral = await _slaRepository.ObtenerDiasUmbralSla(tipoSla);
                    Console.WriteLine($"[ReporteController] Config SLA: código={tipoSla}, umbral={diasUmbral} días");
                }
                catch (InvalidOperationException)
                {
                    return BadRequest(new { error = $"El tipo de SLA '{tipoSla}' no existe en la configuración" });
                }

                // Obtener solicitudes usando el repositorio
                var solicitudes = await _slaRepository.ObtenerSolicitudesPorSla(tipoSla, null, anio, idArea);

                // Filtrar por rango de fechas
                var solicitudesFiltradas = solicitudes
                    .Where(s => s.FechaSolicitud.HasValue &&
                               s.FechaSolicitud.Value >= fechaInicio &&
                               s.FechaSolicitud.Value <= fechaFin)
                    .OrderBy(s => s.FechaSolicitud)
                    .ToList();

                Console.WriteLine($"[ReporteController] Solicitudes encontradas: {solicitudesFiltradas.Count}");

                // Agrupar por mes para la app
                var datosPorMes = solicitudesFiltradas
                    .GroupBy(s => new
                    {
                        Año = s.FechaSolicitud!.Value.Year,
                        Mes = s.FechaSolicitud.Value.Month
                    })
                    .Select(g => new
                    {
                        año = g.Key.Año,
                        mes = g.Key.Mes,
                        mesNombre = new DateTime(g.Key.Año, g.Key.Mes, 1).ToString("MMM yyyy"),
                        totalCasos = g.Count(),
                        cumplidos = g.Count(s => s.NumDiasSla.HasValue && s.NumDiasSla.Value <= diasUmbral),
                        noCumplidos = g.Count(s => !s.NumDiasSla.HasValue || s.NumDiasSla.Value > diasUmbral),
                        porcentajeCumplimiento = g.Any()
                            ? Math.Round((double)g.Count(s => s.NumDiasSla.HasValue && s.NumDiasSla.Value <= diasUmbral) / g.Count() * 100, 2)
                            : 0
                    })
                    .OrderBy(d => d.año)
                    .ThenBy(d => d.mes)
                    .ToList();

                Console.WriteLine($"[ReporteController] Meses con datos: {datosPorMes.Count}");

                return Ok(new
                {
                    // Metadatos
                    tipoSla = tipoSla,
                    diasUmbral = diasUmbral,
                    fechaInicio = fechaInicio.ToString("yyyy-MM-dd"),
                    fechaFin = fechaFin.ToString("yyyy-MM-dd"),
                    totalSolicitudes = solicitudesFiltradas.Count,
                    totalMeses = datosPorMes.Count,

                    // Datos crudos agrupados por mes
                    // LA APP CALCULARÁ: regresión lineal, proyección, tendencia
                    datosMensuales = datosPorMes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReporteController] Error: {ex.Message}");
                Console.WriteLine($"[ReporteController] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = "Error al obtener datos", detalle = ex.Message });
            }
        }
    }
}
