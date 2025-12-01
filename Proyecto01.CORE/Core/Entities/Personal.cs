using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // <-- AÑADE ESTA LÍNEA SI NO LA TIENES
using System.ComponentModel.DataAnnotations;
namespace Proyecto01.CORE.Core.Entities
{
    public class Personal
    {
        [Key]
        public int IdPersonal { get; set; }
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Documento { get; set; }
        public string? Estado { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [NotMapped] // <--- ¡¡¡LA SOLUCIÓN DEFINITIVA!!!
        public DateTime? ActualizadoEn { get; set; }

        // Navegación
        public Usuario? Usuario { get; set; }
        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}