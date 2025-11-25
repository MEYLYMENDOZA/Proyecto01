namespace Proyecto01.CORE.Core.Entities
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Nombre { get; set; }

        public ICollection<RolPermiso>? RolPermisos { get; set; }
    }
}
