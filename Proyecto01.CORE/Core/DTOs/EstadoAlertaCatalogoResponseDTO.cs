namespace Proyecto01.CORE.Core.DTOs
{
    public class EstadoAlertaCatalogoResponseDTO
    {
        public int IdEstadoAlerta { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
