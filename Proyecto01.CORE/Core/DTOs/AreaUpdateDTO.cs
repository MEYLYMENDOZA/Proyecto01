using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class AreaUpdateDTO
    {
        [Required]
        public int IdArea { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string NombreArea { get; set; } = null!;
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
