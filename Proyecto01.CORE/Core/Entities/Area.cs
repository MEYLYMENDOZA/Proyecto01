namespace Proyecto01.CORE.Core.Entities
{
    public class Area
    {
        public int IdArea { get; set; }
        public string NombreArea { get; set; } = null!;
        public string? Descripcion { get; set; }

        // Navegación
        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}
