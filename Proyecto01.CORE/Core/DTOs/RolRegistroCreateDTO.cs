using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class RolRegistroCreateDTO
    {
        [MaxLength(100)]
        public string? BloqueTeach { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        
        public bool EsActivo { get; set; } = true;
        
        [Required]
        [MaxLength(100)]
        public string NombreRol { get; set; } = null!;
    }
}
