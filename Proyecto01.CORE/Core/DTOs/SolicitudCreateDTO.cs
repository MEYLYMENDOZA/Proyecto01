using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class SolicitudCreateDTO
    {
        [Required]
        public int IdPersonal { get; set; }
        
        [Required]
        public int IdRolRegistro { get; set; }
        
        [Required]
        public int IdSla { get; set; }
        
        [Required]
        public int IdArea { get; set; }
        
        [Required]
        public int IdEstadoSolicitud { get; set; }
        
        [MaxLength(500)]
        public string? ResumenSla { get; set; }
        
        [MaxLength(50)]
        public string? OrigenDato { get; set; }
    }
}
