using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Entities;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Core.Services;
using Proyecto01.CORE.Infrastructure.Data;
using System.Globalization;

namespace Proyecto01.CORE.Core.Services
{
    // Ahora C# sí puede encontrar ISolicitudService
    public interface ISolicitudService
    {
        Task<IEnumerable<SolicitudListDTO>> GetAll();
        Task<SolicitudListDTO?> GetById(int id);
        Task<int> Insert(SolicitudCreateDTO dto);
        Task<int> Update(SolicitudUpdateDTO dto);
        Task<bool> Delete(int id);

        // El nuevo método para la carga por lotes
        Task<int> InsertBatch(List<CargaItemDataDto> solicitudesDto);
    }
}