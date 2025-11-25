namespace Proyecto01.CORE.Core.DTOs
{
    public class UsuarioResponseDTO
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public int IdRolSistema { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
