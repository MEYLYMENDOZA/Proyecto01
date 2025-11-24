using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class RolRegistroUpdateDTO
    {
        [Required]
        public int IdRolRegistro { get; set; }
        
        [MaxLength(100)]
        public string? BloqueTeach { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        
        public bool? EsActivo { get; set; }
        
        [MaxLength(100)]
        public string? NombreRol { get; set; }
    }
}
