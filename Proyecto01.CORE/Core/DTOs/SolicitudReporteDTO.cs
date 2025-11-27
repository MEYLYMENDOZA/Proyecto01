using System;

namespace Proyecto01.CORE.Core.DTOs
{
    public class SolicitudReporteDTO
    {
        // --- Campos que ya tienes ---
        public int IdSolicitud { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaIngreso { get; set; } // Útil para la tabla de últimos registros
        public string? ResumenSla { get; set; }

        // --- Campos CRÍTICOS que faltan ---
        public int? NumDiasSla { get; set; } // Días que tardó
        
        // --- Campos anidados para obtener la info relacionada ---
        public ConfigSlaDTO? ConfigSla { get; set; }
        public RolDTO? Rol { get; set; } // O el nombre de tu DTO para el rol
    }
}
