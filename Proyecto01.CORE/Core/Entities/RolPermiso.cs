namespace Proyecto01.CORE.Core.Entities
{
    public class RolPermiso
    {
        public int IdRolSistema { get; set; }
        public int IdPermiso { get; set; }

        public RolesSistema? RolesSistema { get; set; }
        public Permiso? Permiso { get; set; }
    }
}
