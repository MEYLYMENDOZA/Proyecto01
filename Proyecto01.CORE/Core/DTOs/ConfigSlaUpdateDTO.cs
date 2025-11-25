using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class ConfigSlaUpdateDTO
    {
        [Required]
        public int IdSla { get; set; }
        
        [MaxLength(50)]
        public string? CodigoSla { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        
        public int? DiasUmbral { get; set; }
        
        public bool? EsActivo { get; set; }
        
        public int? IdTipoSolicitud { get; set; }
        
        public int? ActualizadoPor { get; set; }
    }
}
