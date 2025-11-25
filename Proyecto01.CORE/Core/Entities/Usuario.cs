namespace Proyecto01.CORE.Core.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public int IdRolSistema { get; set; }
        public int IdEstadoUsuario { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        public DateTime? UltimoLogin { get; set; }

        // Navegación
        public RolesSistema? RolesSistema { get; set; }
        public EstadoUsuarioCatalogo? EstadoUsuario { get; set; }
        public Personal? Personal { get; set; }
        public ICollection<ConfigSla>? ConfigSlasCreadas { get; set; }
        public ICollection<ConfigSla>? ConfigSlasActualizadas { get; set; }
        public ICollection<Solicitud>? SolicitudesCreadas { get; set; }
        public ICollection<Solicitud>? SolicitudesActualizadas { get; set; }
        public ICollection<Reporte>? Reportes { get; set; }
    }
}
