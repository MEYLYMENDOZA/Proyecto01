namespace Proyecto01.CORE.Core.DTOs
{
    public class AlertaResponseDTO
    {
        public int IdAlerta { get; set; }
        public int IdSolicitud { get; set; }
        public int IdTipoAlerta { get; set; }
        public int IdEstadoAlerta { get; set; }
        public string? Nivel { get; set; }
        public string? Mensaje { get; set; }
        public bool? EnviadoEmail { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaLectura { get; set; }
        public DateTime? ActualizadoEn { get; set; }
    }
}
