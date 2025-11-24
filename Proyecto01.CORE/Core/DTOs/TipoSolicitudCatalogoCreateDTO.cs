using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class TipoSolicitudCatalogoCreateDTO
    {
        [Required]
        [MaxLength(50)]
        public string Codigo { get; set; } = null!;
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
