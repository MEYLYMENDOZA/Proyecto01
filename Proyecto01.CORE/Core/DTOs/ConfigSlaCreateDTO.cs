using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class ConfigSlaCreateDTO
    {
        [Required]
        [MaxLength(50)]
        public string CodigoSla { get; set; } = null!;
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        
        public int? DiasUmbral { get; set; }
        
        public bool EsActivo { get; set; } = true;
        
        [Required]
        public int IdTipoSolicitud { get; set; }
        
        public int? CreadoPor { get; set; }
    }
}
