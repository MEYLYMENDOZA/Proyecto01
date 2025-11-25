namespace Proyecto01.CORE.Core.Entities
{
    public class RolesSistema
    {
        public int IdRolSistema { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool EsActivo { get; set; } = true;
        public string? Nombre { get; set; }

        public ICollection<RolPermiso>? RolPermisos { get; set; }
        public ICollection<Usuario>? Usuarios { get; set; }
        public ICollection<ConfigSla>? ConfigSlas { get; set; }
    }
}
