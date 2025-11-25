namespace Proyecto01.CORE.Core.DTOs
{
    public class PersonalListDTO
    {
        public int IdPersonal { get; set; }
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Documento { get; set; }
        public string? Estado { get; set; }
    }
}
