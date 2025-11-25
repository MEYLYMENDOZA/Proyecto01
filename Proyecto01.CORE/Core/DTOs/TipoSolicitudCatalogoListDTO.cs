namespace Proyecto01.CORE.Core.DTOs
{
    public class TipoSolicitudCatalogoListDTO
    {
        public int IdTipoSolicitud { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
