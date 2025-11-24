namespace Proyecto01.CORE.Core.Entities
{
    public class EstadoSolicitudCatalogo
    {
        public int IdEstadoSolicitud { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}
