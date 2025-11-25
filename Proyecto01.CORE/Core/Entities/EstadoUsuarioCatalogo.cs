namespace Proyecto01.CORE.Core.Entities
{
    public class EstadoUsuarioCatalogo
    {
        public int IdEstadoUsuario { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
