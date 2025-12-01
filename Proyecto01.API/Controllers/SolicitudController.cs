using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Linq;

namespace Proyecto01.API.Controllers
{
    [ApiController]
    [Route("api/solicitud")] // La ruta correcta, en singular
    public class SolicitudController : ControllerBase
    {
        // Ahora C# sí puede encontrar ISolicitudService
        private readonly ISolicitudService _service;

        public SolicitudController(ISolicitudService service)
        {
            _service = service;
        }

        // --- TUS MÉTODOS ORIGINALES (GET, POST, PUT, DELETE) ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudListDTO>>> GetAll()
        {
            var solicitudes = await _service.GetAll();
            return Ok(solicitudes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudListDTO>> GetById(int id)
        {
            var solicitud = await _service.GetById(id);
            if (solicitud == null)
                return NotFound(new { message = $"Solicitud con ID {id} no encontrada." });

            return Ok(solicitud);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] SolicitudCreateDTO dto)
        {
            try
            {
                var id = await _service.Insert(dto);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update(int id, [FromBody] SolicitudUpdateDTO dto)
        {
            if (id != dto.IdSolicitud)
                return BadRequest(new { message = "El ID no coincide." });

            try
            {
                var result = await _service.Update(dto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (!result)
                return NotFound(new { message = "Solicitud no encontrada." });

            return Ok(new { message = "Solicitud eliminada." });
        }


        // --- NUEVO MÉTODO PARA LA CARGA POR LOTES ---
        [HttpPost("lote")] // La ruta es -> POST api/solicitud/lote
        public async Task<ActionResult> CreateBatch([FromBody] List<CargaItemDataDto> solicitudesDto)
        {
            if (solicitudesDto == null || !solicitudesDto.Any())
            {
                return BadRequest(new { message = "La lista de solicitudes está vacía." });
            }

            try
            {
                var resultado = await _service.InsertBatch(solicitudesDto);
                return Ok(new { message = $"Se procesaron {resultado} registros con éxito." });
            }
            catch (System.ArgumentException ex)
            {
                // Este error viene de las validaciones que hicimos en el Service
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                // Cualquier otro error inesperado
                return StatusCode(500, new { message = $"Ocurrió un error interno: {ex.Message}" });
            }
        }
    }
}// <-- ESTA ES LA LLAVE FINAL QUE CIERRA EL 'namespace' Y QUE SEGURAMENTE FALTABA