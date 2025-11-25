using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class EstadoSolicitudCatalogoUpdateDTO
    {
        [Required]
        public int IdEstadoSolicitud { get; set; }
        
        [MaxLength(50)]
        public string? Codigo { get; set; }
        
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
