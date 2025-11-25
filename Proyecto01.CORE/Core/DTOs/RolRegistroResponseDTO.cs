namespace Proyecto01.CORE.Core.DTOs
{
    public class RolRegistroResponseDTO
    {
        public int IdRolRegistro { get; set; }
        public string? BloqueTeach { get; set; }
        public string? Descripcion { get; set; }
        public bool EsActivo { get; set; }
        public string NombreRol { get; set; } = null!;
    }
}
