namespace Proyecto01.CORE.Core.DTOs
{
    public class AlertaListDTO
    {
        public int IdAlerta { get; set; }
        public int IdSolicitud { get; set; }
        public string? Nivel { get; set; }
        public string? Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
