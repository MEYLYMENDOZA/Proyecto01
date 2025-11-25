using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoAlertaCatalogoController : ControllerBase
    {
        private readonly ITipoAlertaCatalogoService _tipoAlertaCatalogoService;

        public TipoAlertaCatalogoController(ITipoAlertaCatalogoService tipoAlertaCatalogoService)
        {
            _tipoAlertaCatalogoService = tipoAlertaCatalogoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _tipoAlertaCatalogoService.GetAll();
            return Ok(tipos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _tipoAlertaCatalogoService.GetById(id);
            if (tipo == null)
                return NotFound($"Tipo de Alerta con ID {id} no encontrado.");

            return Ok(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoAlertaCatalogoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _tipoAlertaCatalogoService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TipoAlertaCatalogoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tipoAlertaCatalogoService.Update(dto);
            if (result == 0)
                return NotFound($"Tipo de Alerta con ID {dto.IdTipoAlerta} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tipoAlertaCatalogoService.Delete(id);
            if (!result)
                return NotFound($"Tipo de Alerta con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
