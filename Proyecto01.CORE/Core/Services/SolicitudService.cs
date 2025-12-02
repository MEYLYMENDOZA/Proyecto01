using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.CORE.Core.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _repository;

        public SolicitudService(ISolicitudRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SolicitudListDTO>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<SolicitudListDTO?> GetById(int id)
        {
            if (id <= 0) return null;
            return await _repository.GetById(id);
        }

        // Agrega esto dentro de SolicitudService
        public async Task<int> ProcesarSlas()
        {
            return await _repository.ProcesarSlas();
        }

        public async Task<int> Insert(SolicitudCreateDTO dto)
        {
            if (dto.IdPersonal <= 0)
                throw new ArgumentException("El ID del personal es inválido.");

            if (dto.IdRolRegistro <= 0)
                throw new ArgumentException("El ID del rol de registro es inválido.");

            if (dto.IdSla <= 0)
                throw new ArgumentException("El ID del SLA es inválido.");

            if (dto.IdArea <= 0)
                throw new ArgumentException("El ID del área es inválido.");

            if (dto.IdEstadoSolicitud <= 0)
                throw new ArgumentException("El ID del estado de solicitud es inválido.");

            return await _repository.Insert(dto);
        }

        public async Task<int> Update(SolicitudUpdateDTO dto)
        {
            if (dto.IdSolicitud <= 0)
                throw new ArgumentException("El ID de la solicitud es inválido.");

            var exists = await _repository.Exists(dto.IdSolicitud);
            if (!exists)
                throw new InvalidOperationException($"La solicitud con ID {dto.IdSolicitud} no existe.");

            return await _repository.Update(dto);
        }

        public async Task<bool> Delete(int id)
        {
            if (id <= 0) return false;

            var exists = await _repository.Exists(id);
            if (!exists) return false;

            return await _repository.Delete(id);
        }
    }
}
