namespace Proyecto01.CORE.Core.Entities
{
    public class Solicitud
    {
        public int IdSolicitud { get; set; }
        public int IdPersonal { get; set; }
        public int IdRolRegistro { get; set; }
        public int IdSla { get; set; }
        public int IdArea { get; set; }
        public int IdEstadoSolicitud { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? NumDiasSla { get; set; }
        public string? ResumenSla { get; set; }
        public string? OrigenDato { get; set; }
        public int CreadoPor { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        public int? ActualizadoPor { get; set; }

        // Navegación
        public Personal? Personal { get; set; }
        public RolRegistro? RolRegistro { get; set; }
        public ConfigSla? ConfigSla { get; set; }
        public Area? Area { get; set; }
        public EstadoSolicitudCatalogo? EstadoSolicitud { get; set; }
        public Usuario? UsuarioCreadoPor { get; set; }
        public Usuario? UsuarioActualizadoPor { get; set; }
        public ICollection<Alerta>? Alertas { get; set; }
        public ICollection<ReporteDetalle>? ReporteDetalles { get; set; }
    }
}
