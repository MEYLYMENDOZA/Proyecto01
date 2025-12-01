using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using System.Globalization;
using Proyecto01.CORE.Infrastructure.Data;
namespace Proyecto01.CORE.Core.Services
{
    public class SolicitudService : ISolicitudService // <-- Implementa la interfaz limpia
    {
        private readonly ISolicitudRepository _repository;
        private readonly Proyecto01DbContext _context;

        public SolicitudService(ISolicitudRepository repository, Proyecto01DbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // --- TUS MÉTODOS ORIGINALES ---
        public async Task<IEnumerable<SolicitudListDTO>> GetAll() => await _repository.GetAll();
        public async Task<SolicitudListDTO?> GetById(int id) => await _repository.GetById(id);
        public async Task<int> Insert(SolicitudCreateDTO dto) => await _repository.Insert(dto);
        public async Task<int> Update(SolicitudUpdateDTO dto) => await _repository.Update(dto);
        public async Task<bool> Delete(int id) => await _repository.Delete(id);

        // --- NUEVO MÉTODO ---
        public async Task<int> InsertBatch(List<CargaItemDataDto> solicitudesDto)
        {
            var nuevasSolicitudesDb = new List<Solicitud>();
            int usuarioLogueadoId = 2; // OJO: VALOR DE EJEMPLO

            foreach (var dto in solicitudesDto)
            {
                // --- INICIO DE LA MODIFICACIÓN ---
                // Optimizamos las consultas para buscar solo el ID y evitar errores de EF.

                var rolId = await _context.RolesRegistro
                    .Where(r => r.NombreRol.ToLower() == dto.Rol.ToLower())
                    .Select(r => (int?)r.IdRolRegistro)
                    .FirstOrDefaultAsync();

                var slaId = await _context.ConfigSlas
                    .Where(s => s.CodigoSla.ToLower() == dto.TipoSla.ToLower())
                    .Select(s => (int?)s.IdSla)
                    .FirstOrDefaultAsync();

                var estadoId = await _context.EstadosSolicitud
                    .Where(e => e.Descripcion.ToLower() == dto.Estado.ToLower())
                    .Select(e => (int?)e.IdEstadoSolicitud)
                    .FirstOrDefaultAsync();

                // --- FIN DE LA MODIFICACIÓN ---

                if (rolId == null)
                    throw new ArgumentException($"El rol '{dto.Rol}' del registro '{dto.Codigo}' no fue encontrado.");
                if (slaId == null)
                    throw new ArgumentException($"El TipoSla '{dto.TipoSla}' del registro '{dto.Codigo}' no fue encontrado. Revisa que el código en la BD coincida (ej: 'SLA001').");
                if (estadoId == null)
                    throw new ArgumentException($"El estado '{dto.Estado}' del registro '{dto.Codigo}' no fue encontrado.");

                if (!DateTime.TryParseExact(dto.FechaSolicitud, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaSolicitud))
                    throw new ArgumentException($"Formato de fecha inválido para {dto.Codigo}");

                if (!DateTime.TryParseExact(dto.FechaIngreso, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaIngreso))
                    throw new ArgumentException($"Formato de fecha inválido para {dto.Codigo}");

                var nuevaSolicitudDb = new Solicitud
                {
                    FechaSolicitud = fechaSolicitud,
                    FechaIngreso = fechaIngreso,
                    NumDiasSla = dto.DiasTranscurridos,
                    IdRolRegistro = rolId.Value, // Usamos el ID obtenido
                    IdSla = slaId.Value,         // Usamos el ID obtenido
                    IdEstadoSolicitud = estadoId.Value, // Usamos el ID obtenido
                    OrigenDato = "APP",
                    CreadoEn = DateTime.UtcNow,
                    CreadoPor = usuarioLogueadoId,
                    IdPersonal = 2, // VALOR DE EJEMPLO
                    IdArea = 1,     // VALOR DE EJEMPLO
                    ResumenSla = $"Carga desde APP: {dto.Codigo}"
                };
                nuevasSolicitudesDb.Add(nuevaSolicitudDb);
            }

            await _context.Solicitudes.AddRangeAsync(nuevasSolicitudesDb);
            return await _context.SaveChangesAsync();
        }
    }
}