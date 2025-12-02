using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class UsuarioCreateDTO
    {
        // ========================================
        // CAMPOS DE LA TABLA USUARIO
        // ========================================
        
        // Validación de Username
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
        public string Username { get; set; } = null!;

        // Validación de Correo
        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(100, ErrorMessage = "El correo no debe exceder los 100 caracteres.")]
        public string Correo { get; set; } = null!;

        // Validación de Contraseña
        [Required(ErrorMessage = "La contraseña es requerida.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = null!;

        // Validación de Clave Foránea: Rol
        [Required(ErrorMessage = "El rol de sistema es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe especificar un rol de sistema válido.")]
        public int IdRolSistema { get; set; }

        // ========================================
        // CAMPOS DE LA TABLA PERSONAL
        // ========================================

        // Nombres (Requerido)
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(120, ErrorMessage = "El nombre no debe exceder los 120 caracteres.")]
        public string Nombres { get; set; } = null!;

        // Apellidos (Requerido)
        [Required(ErrorMessage = "El apellido es requerido.")]
        [StringLength(120, ErrorMessage = "El apellido no debe exceder los 120 caracteres.")]
        public string Apellidos { get; set; } = null!;

        // Documento (Opcional)
        [StringLength(20, ErrorMessage = "El documento no debe exceder los 20 caracteres.")]
        public string? Documento { get; set; }

        // ========================================
        // CAMPOS INTERNOS (No enviados por Android)
        // ========================================
        
        // Campo para uso interno (Hash de la contraseña) - Generado por el Service
        [System.Text.Json.Serialization.JsonIgnore]
        public string? PasswordHash { get; set; }

        // Estado del Usuario - Asignado automáticamente al crear
        [System.Text.Json.Serialization.JsonIgnore]
        public int IdEstadoUsuario { get; set; } = 1; // ACTIVO por defecto
    }
}