namespace Proyecto01.CORE.Core.DTOs
{
    public class ConfigSlaResponseDTO
    {
        public int IdSla { get; set; }
        public string CodigoSla { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int? DiasUmbral { get; set; }
        public bool EsActivo { get; set; }
        public int IdTipoSolicitud { get; set; }
        public DateTime CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public int? CreadoPor { get; set; }
        public int? ActualizadoPor { get; set; }
    }
}
