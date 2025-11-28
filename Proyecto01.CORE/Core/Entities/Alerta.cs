using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // NECESARIO PARA [Column] y [Table]

namespace Proyecto01.CORE.Core.Entities
{
    [Table("alerta")] // Asegura que busque la tabla 'alerta'
    public class Alerta
    {
        [Key]
        [Column("id_alerta")] // Mapea a la columna SQL correcta
        public int IdAlerta { get; set; }

        [Column("id_solicitud")]
        public int IdSolicitud { get; set; }

        [Column("id_tipo_alerta")]
        public int IdTipoAlerta { get; set; }

        [Column("id_estado_alerta")]
        public int IdEstadoAlerta { get; set; }

        [Column("nivel")]
        public string? Nivel { get; set; }

        [Column("mensaje")]
        public string? Mensaje { get; set; }

        [Column("enviado_email")] // <--- AQUÍ ESTABA EL ERROR
        public bool? EnviadoEmail { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fecha_lectura")]
        public DateTime? FechaLectura { get; set; }

        [Column("actualizado_en")] // <--- AQUÍ ESTABA EL OTRO ERROR
        public DateTime? ActualizadoEn { get; set; }

        // Propiedades de Navegación (Relaciones)
        // A veces es necesario especificar la llave foránea explícitamente
        [ForeignKey("IdSolicitud")]
        public Solicitud? Solicitud { get; set; }

        [ForeignKey("IdTipoAlerta")]
        public TipoAlertaCatalogo? TipoAlerta { get; set; }

        [ForeignKey("IdEstadoAlerta")]
        public EstadoAlertaCatalogo? EstadoAlerta { get; set; }
    }
}