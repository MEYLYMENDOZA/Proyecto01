using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class PersonalCreateDTO
    {
        [Required]
        public int IdUsuario { get; set; }
        
        [MaxLength(100)]
        public string? Nombres { get; set; }
        
        [MaxLength(100)]
        public string? Apellidos { get; set; }
        
        [MaxLength(20)]
        public string? Documento { get; set; }
    }
}
