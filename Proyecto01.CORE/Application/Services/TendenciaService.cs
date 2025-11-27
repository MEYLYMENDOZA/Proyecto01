using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Application.Services
{
    public class TendenciaService
    {
        private readonly ISlaRepository _slaRepository;
        private readonly ITendenciaLogRepository _tendenciaLogRepository;

        public TendenciaService(
            ISlaRepository slaRepository,
            ITendenciaLogRepository tendenciaLogRepository)
        {
            _slaRepository = slaRepository;
            _tendenciaLogRepository = tendenciaLogRepository;
        }

        public async Task<ReporteTendenciaDTO> GenerarReporteTendencia(
            int? mes = null,
            int? anio = null,
            string? tipoSla = null,
            int? idArea = null,
            string? usuarioSolicitante = null,
            string? ipCliente = null)
        {
            // Validar parámetros
            if (mes.HasValue && (mes.Value < 1 || mes.Value > 12))
            {
                throw new ArgumentException("El mes debe estar entre 1 y 12", nameof(mes));
            }

            if (anio.HasValue && anio.Value < 2000)
            {
                throw new ArgumentException("El año debe ser mayor o igual a 2000", nameof(anio));
            }

            // Validar tipo SLA - permitir cualquier código de ConfigSla
            if (string.IsNullOrWhiteSpace(tipoSla))
            {
                throw new ArgumentException("El tipo de SLA es obligatorio", nameof(tipoSla));
            }

            // Sanitizar entrada (OWASP)
            tipoSla = tipoSla.Trim().ToUpperInvariant();

            // No validar códigos específicos aquí
            // La validación real ocurre en la consulta SQL cuando filtra por CodigoSla
            Console.WriteLine($"[TendenciaService] Tipo SLA recibido: {tipoSla}");

            // Si no se especifica mes/año, usar el actual
            var fechaAnalisis = DateTime.Now;
            var mesAnalisis = mes ?? fechaAnalisis.Month;
            var anioAnalisis = anio ?? fechaAnalisis.Year;

            Console.WriteLine($"[TendenciaService] Buscando solicitudes - Tipo SLA: {tipoSla}, Mes: {mesAnalisis}, Año: {anioAnalisis}, Área: {idArea}");

            // Obtener solicitudes
            var solicitudes = await _slaRepository.ObtenerSolicitudesPorSla(
                tipoSla, mesAnalisis, anioAnalisis, idArea);

            var listaSolicitudes = solicitudes.ToList();
            Console.WriteLine($"[TendenciaService] Solicitudes encontradas: {listaSolicitudes.Count}");

            // Obtener días umbral del SLA
            int diasUmbral = 0;
            try
            {
                diasUmbral = await _slaRepository.ObtenerDiasUmbralSla(tipoSla);
                Console.WriteLine($"[TendenciaService] Días umbral para {tipoSla}: {diasUmbral}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TendenciaService] Error al obtener días umbral: {ex.Message}");
                throw new InvalidOperationException($"No se encontró configuración SLA para: {tipoSla}", ex);
            }

            if (!listaSolicitudes.Any())
            {
                Console.WriteLine($"[TendenciaService] No hay datos para el período solicitado. Retornando reporte vacío.");
                // Retornar reporte vacío en lugar de error
                return new ReporteTendenciaDTO
                {
                    Historico = new List<HistoricoDTO>(),
                    Tendencia = new List<decimal>(),
                    Proyeccion = 0,
                    Pendiente = 0,
                    Intercepto = 0,
                    EstadoTendencia = "SIN_DATOS",
                    TotalRegistros = 0,
                    FechaGeneracion = fechaAnalisis
                };
            }

            // Calcular cumplimiento
            var totalSolicitudes = listaSolicitudes.Count;
            var cumplenSla = listaSolicitudes.Count(s => 
                s.NumDiasSla.HasValue && s.NumDiasSla.Value <= diasUmbral);

            var porcentajeCumplimiento = totalSolicitudes > 0
                ? (decimal)cumplenSla / totalSolicitudes * 100
                : 0;

            // Obtener historial para calcular tendencia
            var historial = await _tendenciaLogRepository.ObtenerHistorial(
                tipoSla, idArea, limite: 12);

            var listaHistorial = historial.ToList();

            // Calcular regresión lineal para proyección
            var (pendiente, intercepto, proyeccion) = CalcularRegresionLineal(listaHistorial, porcentajeCumplimiento);

            // Determinar estado de tendencia
            var estadoTendencia = DeterminarEstadoTendencia(pendiente, proyeccion);

            // Guardar log
            var log = new PrediccionTendenciaLog
            {
                TipoSla = tipoSla,
                IdArea = idArea,
                FechaAnalisis = fechaAnalisis,
                MesAnalisis = mesAnalisis,
                AnioAnalisis = anioAnalisis,
                TotalSolicitudes = totalSolicitudes,
                CumplenSla = cumplenSla,
                PorcentajeCumplimiento = porcentajeCumplimiento,
                ProyeccionMesSiguiente = proyeccion,
                TendenciaEstado = estadoTendencia,
                UsuarioSolicitante = usuarioSolicitante,
                IpCliente = ipCliente,
                CreadoEn = DateTime.Now
            };

            await _tendenciaLogRepository.GuardarLog(log);

            // Preparar respuesta
            return new ReporteTendenciaDTO
            {
                Historico = listaHistorial.Select(h => new HistoricoDTO
                {
                    Mes = h.MesAnalisis,
                    Anio = h.AnioAnalisis,
                    PorcentajeCumplimiento = h.PorcentajeCumplimiento
                }).Reverse().ToList(),
                Tendencia = listaHistorial.Select(h => h.PorcentajeCumplimiento).Reverse().ToList(),
                Proyeccion = proyeccion,
                Pendiente = pendiente,
                Intercepto = intercepto,
                EstadoTendencia = estadoTendencia,
                TotalRegistros = totalSolicitudes,
                FechaGeneracion = fechaAnalisis
            };
        }

        private (decimal pendiente, decimal intercepto, decimal proyeccion) CalcularRegresionLineal(
            List<PrediccionTendenciaLog> historial,
            decimal cumplimientoActual)
        {
            if (!historial.Any())
            {
                // Sin historial, proyectar el valor actual
                return (0, cumplimientoActual, cumplimientoActual);
            }

            // Agregar el punto actual al historial para el cálculo
            var datosCompletos = historial
                .Select((h, i) => new { X = (decimal)(historial.Count - i), Y = h.PorcentajeCumplimiento })
                .ToList();

            // Agregar el punto actual
            datosCompletos.Insert(0, new { X = (decimal)(datosCompletos.Count + 1), Y = cumplimientoActual });

            var n = datosCompletos.Count;

            if (n < 2)
            {
                return (0, cumplimientoActual, cumplimientoActual);
            }

            // Calcular regresión lineal: y = mx + b
            var sumX = datosCompletos.Sum(d => d.X);
            var sumY = datosCompletos.Sum(d => d.Y);
            var sumXY = datosCompletos.Sum(d => d.X * d.Y);
            var sumX2 = datosCompletos.Sum(d => d.X * d.X);

            var pendiente = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            var intercepto = (sumY - pendiente * sumX) / n;

            // Proyectar para el siguiente mes
            var xProyeccion = (decimal)(n + 1);
            var proyeccion = pendiente * xProyeccion + intercepto;

            // Asegurar que la proyección esté en rango válido [0, 100]
            proyeccion = Math.Max(0, Math.Min(100, proyeccion));

            return (Math.Round(pendiente, 4), Math.Round(intercepto, 2), Math.Round(proyeccion, 2));
        }

        private string DeterminarEstadoTendencia(decimal pendiente, decimal proyeccion)
        {
            if (pendiente > 0.5m)
                return "MEJORANDO";
            else if (pendiente < -0.5m)
                return "EMPEORANDO";
            else if (proyeccion >= 80)
                return "ESTABLE_ALTO";
            else if (proyeccion >= 60)
                return "ESTABLE_MEDIO";
            else
                return "ESTABLE_BAJO";
        }
    }

    // DTOs para el servicio
    public class ReporteTendenciaDTO
    {
        public List<HistoricoDTO> Historico { get; set; } = new();
        public List<decimal> Tendencia { get; set; } = new();
        public decimal Proyeccion { get; set; }
        public decimal Pendiente { get; set; }
        public decimal Intercepto { get; set; }
        public string EstadoTendencia { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
        public DateTime FechaGeneracion { get; set; }
    }

    public class HistoricoDTO
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal PorcentajeCumplimiento { get; set; }
    }
}
