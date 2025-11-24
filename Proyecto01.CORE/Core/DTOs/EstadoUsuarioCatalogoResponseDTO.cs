namespace Proyecto01.CORE.Core.DTOs
{
    public class EstadoUsuarioCatalogoResponseDTO
    {
        public int IdEstadoUsuario { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
