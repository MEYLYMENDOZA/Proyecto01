namespace Proyecto01.CORE.Core.DTOs
{
    public class PermisoResponseDTO
    {
        public int IdPermiso { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Nombre { get; set; }
    }
}
