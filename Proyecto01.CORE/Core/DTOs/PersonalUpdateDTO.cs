using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class PersonalUpdateDTO
    {
        [Required]
        public int IdPersonal { get; set; }
        
        [MaxLength(100)]
        public string? Nombres { get; set; }
        
        [MaxLength(100)]
        public string? Apellidos { get; set; }
        
        [MaxLength(20)]
        public string? Documento { get; set; }
        
        [MaxLength(20)]
        public string? Estado { get; set; }
    }
}
