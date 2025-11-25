using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class RolesSistemaCreateDTO
    {
        [Required]
        [MaxLength(50)]
        public string Codigo { get; set; } = null!;
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        
        public bool EsActivo { get; set; } = true;
        
        [MaxLength(100)]
        public string? Nombre { get; set; }
    }
}
