using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class AlertaCreateDTO
    {
        [Required]
        public int IdSolicitud { get; set; }
        
        [Required]
        public int IdTipoAlerta { get; set; }
        
        [Required]
        public int IdEstadoAlerta { get; set; }
        
        [MaxLength(50)]
        public string? Nivel { get; set; }
        
        [MaxLength(500)]
        public string? Mensaje { get; set; }
        
        public bool? EnviadoEmail { get; set; }
    }
}
