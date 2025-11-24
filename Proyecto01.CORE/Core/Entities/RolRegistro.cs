namespace Proyecto01.CORE.Core.Entities
{
    public class RolRegistro
    {
        public int IdRolRegistro { get; set; }
        public string? BloqueTeach { get; set; }
        public string? Descripcion { get; set; }
        public bool EsActivo { get; set; } = true;
        public string NombreRol { get; set; } = null!;

        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}
