using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoSolicitudCatalogoController : ControllerBase
    {
        private readonly IEstadoSolicitudCatalogoService _estadoSolicitudCatalogoService;

        public EstadoSolicitudCatalogoController(IEstadoSolicitudCatalogoService estadoSolicitudCatalogoService)
        {
            _estadoSolicitudCatalogoService = estadoSolicitudCatalogoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var estados = await _estadoSolicitudCatalogoService.GetAll();
            return Ok(estados);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var estado = await _estadoSolicitudCatalogoService.GetById(id);
            if (estado == null)
                return NotFound($"Estado de Solicitud con ID {id} no encontrado.");

            return Ok(estado);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EstadoSolicitudCatalogoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _estadoSolicitudCatalogoService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EstadoSolicitudCatalogoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _estadoSolicitudCatalogoService.Update(dto);
            if (result == 0)
                return NotFound($"Estado de Solicitud con ID {dto.IdEstadoSolicitud} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estadoSolicitudCatalogoService.Delete(id);
            if (!result)
                return NotFound($"Estado de Solicitud con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
