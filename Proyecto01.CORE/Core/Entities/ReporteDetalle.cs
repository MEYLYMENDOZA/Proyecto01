namespace Proyecto01.CORE.Core.Entities
{
    public class ReporteDetalle
    {
        public int IdReporte { get; set; }
        public int IdSolicitud { get; set; }

        // Navegación
        public Reporte? Reporte { get; set; }
        public Solicitud? Solicitud { get; set; }
    }
}
