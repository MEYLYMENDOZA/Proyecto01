namespace Proyecto01.CORE.Core.DTOs
{
    public class RolesSistemaListDTO
    {
        public int IdRolSistema { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Nombre { get; set; }
        public bool EsActivo { get; set; }
    }
}
