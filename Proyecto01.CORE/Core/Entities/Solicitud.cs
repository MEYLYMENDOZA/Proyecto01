using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Proyecto01.CORE.Core.Entities
{
    // --- VERSIÓN LIMPIA Y DEFINITIVA ---
    // Sin atributos [Table] ni [Column] para eliminar el conflicto.
    // La configuración vive 100% en Proyecto01DbContext.cs, que es lo correcto.
    public class Solicitud
    {
        [Key] // El [Key] es el único que necesitamos aquí.
        public int IdSolicitud { get; set; }
        public int IdPersonal { get; set; }
        public int IdRolRegistro { get; set; }
        public int IdSla { get; set; }
        public int IdArea { get; set; }
        public int IdEstadoSolicitud { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? NumDiasSla { get; set; }
        public string? ResumenSla { get; set; }
        public string? OrigenDato { get; set; }
        public int CreadoPor { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizadoEn { get; set; }
        public int? ActualizadoPor { get; set; }

        // --- Navegación (esto no cambia y está perfecto) ---
        public Personal? Personal { get; set; }
        public RolRegistro? RolRegistro { get; set; }
        public ConfigSla? ConfigSla { get; set; }
        public Area? Area { get; set; }
        public EstadoSolicitudCatalogo? EstadoSolicitud { get; set; }
        public Usuario? UsuarioCreadoPor { get; set; }
        public Usuario? UsuarioActualizadoPor { get; set; }
        public ICollection<Alerta>? Alertas { get; set; }
        public ICollection<ReporteDetalle>? ReporteDetalles { get; set; }
    }
}