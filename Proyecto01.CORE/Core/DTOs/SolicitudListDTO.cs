namespace Proyecto01.CORE.Core.DTOs
{
    public class SolicitudListDTO
    {
        public int IdSolicitud { get; set; }
        public int IdPersonal { get; set; }
        public int IdArea { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public string? ResumenSla { get; set; }
    }
}
