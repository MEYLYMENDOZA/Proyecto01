namespace Proyecto01.CORE.Core.DTOs
{
    public class TipoAlertaCatalogoResponseDTO
    {
        public int IdTipoAlerta { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
