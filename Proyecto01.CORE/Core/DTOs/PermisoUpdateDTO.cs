using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class PermisoUpdateDTO
    {
        [Required]
        public int IdPermiso { get; set; }
        
        [MaxLength(50)]
        public string? Codigo { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        
        [MaxLength(100)]
        public string? Nombre { get; set; }
    }
}
