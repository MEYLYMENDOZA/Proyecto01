namespace Proyecto01.CORE.Core.DTOs
{
    public class TipoSolicitudCatalogoResponseDTO
    {
        public int IdTipoSolicitud { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
