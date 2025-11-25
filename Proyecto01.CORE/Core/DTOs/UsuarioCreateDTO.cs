using System.ComponentModel.DataAnnotations;

namespace Proyecto01.CORE.Core.DTOs
{
    public class UsuarioCreateDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "El correo es requerido.")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get; set; } = null!;

        public string? PasswordHash { get; set; }

        public int IdRolSistema { get; set; }

        public int IdEstadoUsuario { get; set; } = 1; // Default to active state
    }
}
