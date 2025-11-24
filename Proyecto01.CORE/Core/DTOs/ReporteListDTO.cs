namespace Proyecto01.CORE.Core.DTOs
{
    public class ReporteListDTO
    {
        public int IdReporte { get; set; }
        public string? TipoReporte { get; set; }
        public string? Formato { get; set; }
        public DateTime FechaGeneracion { get; set; }
    }
}
