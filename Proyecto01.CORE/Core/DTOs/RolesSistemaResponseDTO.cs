namespace Proyecto01.CORE.Core.DTOs
{
    public class RolesSistemaResponseDTO
    {
        public int IdRolSistema { get; set; }
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool EsActivo { get; set; }
        public string? Nombre { get; set; }
    }
}
