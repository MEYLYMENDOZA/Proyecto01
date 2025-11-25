namespace Proyecto01.CORE.Core.Entities
{
    public class TipoAlertaCatalogo
    {
        public int IdTipoAlerta { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<Alerta>? Alertas { get; set; }
    }
}
