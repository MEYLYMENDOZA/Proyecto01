using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class TipoAlertaCatalogoUpdateDTO
    {
        [Required]
        public int IdTipoAlerta { get; set; }
        
        [MaxLength(50)]
        public string? Codigo { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
