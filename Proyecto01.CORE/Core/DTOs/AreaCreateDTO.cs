using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class AreaCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string NombreArea { get; set; } = null!;
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
