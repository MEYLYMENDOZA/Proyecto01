namespace Proyecto01.CORE.Core.DTOs
{
    public class EstadoSolicitudCatalogoListDTO
    {
        public int IdEstadoSolicitud { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
