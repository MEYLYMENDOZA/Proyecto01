namespace Proyecto01.CORE.Core.Entities
{
    public class EstadoAlertaCatalogo
    {
        public int IdEstadoAlerta { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<Alerta>? Alertas { get; set; }
    }
}
