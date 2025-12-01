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

            // --- PASO 1: Obtener el ID del usuario logueado (temporalmente sigue siendo 2) ---
            int usuarioLogueadoId = 1; // OJO: VALOR DE EJEMPLO

            // --- PASO 2: Buscar el registro de Personal asociado a este usuario ---
            var personalDelUsuario = await _context.Personales
                                             .FirstOrDefaultAsync(p => p.IdUsuario == usuarioLogueadoId);

            // Si no se encuentra un registro de personal para el usuario, no se puede continuar.
            if (personalDelUsuario == null)
            {
                // En una aplicación real, aquí se manejaría un error más elegante.
                throw new InvalidOperationException($"No se encontró un registro de 'Personal' para el usuario con ID {usuarioLogueadoId}. Asegúrate de que el usuario esté registrado en la tabla de Personal.");
            }

            // --- PASO 3: Usar el IdPersonal encontrado en todos los nuevos registros ---
            foreach (var dto in solicitudesDto)
            {
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

                if (rolId == null)
                    throw new ArgumentException($"El rol '{dto.Rol}' del registro '{dto.Codigo}' no fue encontrado.");
                if (slaId == null)
                    throw new ArgumentException($"El TipoSla '{dto.TipoSla}' del registro '{dto.Codigo}' no fue encontrado.");
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
                    IdRolRegistro = rolId.Value,
                    IdSla = slaId.Value,
                    IdEstadoSolicitud = estadoId.Value,
                    OrigenDato = "APP",
                    CreadoEn = DateTime.UtcNow,

                    // --- VALORES DINÁMICOS ---
                    CreadoPor = usuarioLogueadoId,
                    IdPersonal = personalDelUsuario.IdPersonal, // ¡YA NO ES UN VALOR QUEMADO!

                    // --- VALORES AÚN PENDIENTES ---
                    IdArea = 1,     // OJO: VALOR DE EJEMPLO. El siguiente paso sería asociar un Área al Personal.
                    ResumenSla = $"Carga desde APP: {dto.Codigo}"
                };
                nuevasSolicitudesDb.Add(nuevaSolicitudDb);
            }

            await _context.Solicitudes.AddRangeAsync(nuevasSolicitudesDb);
            return await _context.SaveChangesAsync();
        }
    }
}