using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class ReporteCreateDTO
    {
        [MaxLength(50)]
        public string? TipoReporte { get; set; }
        
        [MaxLength(20)]
        public string? Formato { get; set; }
        
        public string? FiltrosJson { get; set; }
        
        [Required]
        public int GeneradoPor { get; set; }
        
        [MaxLength(500)]
        public string? RutaArchivo { get; set; }
    }
}
