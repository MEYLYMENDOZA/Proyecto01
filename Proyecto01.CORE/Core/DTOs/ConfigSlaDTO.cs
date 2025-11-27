
namespace Proyecto01.CORE.Core.DTOs
{
    // DTO para la configuración del SLA
    public class ConfigSlaDTO 
    {
        public string CodigoSla { get; set; } // ej: "SLA-TI"
        public int? DiasUmbral { get; set; }  // Límite de días
    }
}
