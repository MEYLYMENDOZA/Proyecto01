using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class SolicitudUpdateDTO
    {
        [Required]
        public int IdSolicitud { get; set; }
        
        public int? IdEstadoSolicitud { get; set; }
        
        public int? NumDiasSla { get; set; }
    }
}
