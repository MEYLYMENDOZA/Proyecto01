using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class UsuarioUpdateDTO
    {
        // Identificador del usuario (requerido para saber qué usuario actualizar)
        [Required(ErrorMessage = "El ID del usuario es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del usuario debe ser válido.")]
        public int IdUsuario { get; set; }

        // Validación de Username
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
        public string Username { get; set; } = null!;

        // Validación de Correo
        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(100, ErrorMessage = "El correo no debe exceder los 100 caracteres.")]
        public string Correo { get; set; } = null!;

        // Password es OPCIONAL (solo se envía si se desea cambiar)
        [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string? Password { get; set; }

        // Validación de Clave Foránea: Rol
        [Required(ErrorMessage = "El rol de sistema es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe especificar un rol de sistema válido.")]
        public int IdRolSistema { get; set; }

        // Validación de Clave Foránea: Estado de Usuario
        [Required(ErrorMessage = "El estado de usuario es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe especificar un estado de usuario válido.")]
        public int IdEstadoUsuario { get; set; }
    }
}
