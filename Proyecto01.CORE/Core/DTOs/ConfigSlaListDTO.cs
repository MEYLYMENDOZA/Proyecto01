namespace Proyecto01.CORE.Core.DTOs
{
    public class ConfigSlaListDTO
    {
        public int IdSla { get; set; }
        public string CodigoSla { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int? DiasUmbral { get; set; }
        public bool EsActivo { get; set; }
    }
}
