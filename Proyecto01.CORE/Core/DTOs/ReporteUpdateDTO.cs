using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class ReporteUpdateDTO
    {
        [Required]
        public int IdReporte { get; set; }
        
        [MaxLength(500)]
        public string? RutaArchivo { get; set; }
    }
}
