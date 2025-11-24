using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class AlertaUpdateDTO
    {
        [Required]
        public int IdAlerta { get; set; }
        
        public int? IdEstadoAlerta { get; set; }
        
        [MaxLength(50)]
        public string? Nivel { get; set; }
        
        [MaxLength(500)]
        public string? Mensaje { get; set; }
        
        public bool? EnviadoEmail { get; set; }
        
        public DateTime? FechaLectura { get; set; }
    }
}
