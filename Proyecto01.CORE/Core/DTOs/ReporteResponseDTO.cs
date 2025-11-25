namespace Proyecto01.CORE.Core.DTOs
{
    public class ReporteResponseDTO
    {
        public int IdReporte { get; set; }
        public string? TipoReporte { get; set; }
        public string? Formato { get; set; }
        public string? FiltrosJson { get; set; }
        public int GeneradoPor { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string? RutaArchivo { get; set; }
    }
}
