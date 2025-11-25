namespace Proyecto01.CORE.Core.Entities
{
    public class TipoSolicitudCatalogo
    {
        public int IdTipoSolicitud { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<ConfigSla>? ConfigSlas { get; set; }
    }
}
