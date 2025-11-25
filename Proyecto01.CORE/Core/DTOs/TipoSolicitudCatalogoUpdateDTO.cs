using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class TipoSolicitudCatalogoUpdateDTO
    {
        [Required]
        public int IdTipoSolicitud { get; set; }
        
        [MaxLength(50)]
        public string? Codigo { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
