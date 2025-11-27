namespace Proyecto01.CORE.Core.Entities
{
    public class PrediccionTendenciaLog
    {
        public int IdLog { get; set; }
        public string TipoSla { get; set; } = null!;
        public int? IdArea { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public int MesAnalisis { get; set; }
        public int AnioAnalisis { get; set; }
        public int TotalSolicitudes { get; set; }
        public int CumplenSla { get; set; }
        public decimal PorcentajeCumplimiento { get; set; }
        public decimal? ProyeccionMesSiguiente { get; set; }
        public string? TendenciaEstado { get; set; }
        public string? UsuarioSolicitante { get; set; }
        public string? IpCliente { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
