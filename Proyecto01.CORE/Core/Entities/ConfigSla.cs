namespace Proyecto01.CORE.Core.Entities
{
    public class ConfigSla
    {
        public int IdSla { get; set; }
        public string CodigoSla { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int? DiasUmbral { get; set; }
        public bool EsActivo { get; set; } = true;
        public int IdTipoSolicitud { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        public int? CreadoPor { get; set; }
        public int? ActualizadoPor { get; set; }

        // Navegación
        public TipoSolicitudCatalogo? TipoSolicitud { get; set; }
        public Usuario? UsuarioCreadoPor { get; set; }
        public Usuario? UsuarioActualizadoPor { get; set; }
        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}
