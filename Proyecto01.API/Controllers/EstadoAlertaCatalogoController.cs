using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoAlertaCatalogoController : ControllerBase
    {
        private readonly IEstadoAlertaCatalogoService _estadoAlertaCatalogoService;

        public EstadoAlertaCatalogoController(IEstadoAlertaCatalogoService estadoAlertaCatalogoService)
        {
            _estadoAlertaCatalogoService = estadoAlertaCatalogoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var estados = await _estadoAlertaCatalogoService.GetAll();
            return Ok(estados);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var estado = await _estadoAlertaCatalogoService.GetById(id);
            if (estado == null)
                return NotFound($"Estado de Alerta con ID {id} no encontrado.");

            return Ok(estado);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EstadoAlertaCatalogoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _estadoAlertaCatalogoService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EstadoAlertaCatalogoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _estadoAlertaCatalogoService.Update(dto);
            if (result == 0)
                return NotFound($"Estado de Alerta con ID {dto.IdEstadoAlerta} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estadoAlertaCatalogoService.Delete(id);
            if (!result)
                return NotFound($"Estado de Alerta con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
