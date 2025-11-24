using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class RolesSistemaUpdateDTO
    {
        [Required]
        public int IdRolSistema { get; set; }
        
        [MaxLength(50)]
        public string? Codigo { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        
        public bool? EsActivo { get; set; }
        
        [MaxLength(100)]
        public string? Nombre { get; set; }
    }
}
