namespace Proyecto01.CORE.Core.Entities
{
    public class Reporte
    {
        public int IdReporte { get; set; }
        public string? TipoReporte { get; set; }
        public string? Formato { get; set; }
        public string? FiltrosJson { get; set; }
        public int GeneradoPor { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
        public string? RutaArchivo { get; set; }

        // Navegación
        public Usuario? Usuario { get; set; }
        public ICollection<ReporteDetalle>? ReporteDetalles { get; set; }
    }
}
