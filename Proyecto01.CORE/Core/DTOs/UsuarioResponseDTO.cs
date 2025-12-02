namespace Proyecto01.CORE.Core.DTOs
{
    public class UsuarioResponseDTO
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = null!;
        public string Correo { get; set; } = null!;

        // ¡IMPORTANTE! Agregamos el hash aquí para que el Repositorio lo devuelva.
        // Lo marcamos como opcional (?), y el servicio debe ponerlo en null antes de devolverlo.
        public string? PasswordHash { get; set; }

        public int IdRolSistema { get; set; }
        public int IdEstadoUsuario { get; set; }
        public DateTime CreadoEn { get; set; }
        // ... (otros campos si los tienes) ...
    }
}