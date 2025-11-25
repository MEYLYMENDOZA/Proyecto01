using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertaController : ControllerBase
    {
        private readonly IAlertaService _alertaService;

        public AlertaController(IAlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var alertas = await _alertaService.GetAll();
            return Ok(alertas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alerta = await _alertaService.GetById(id);
            if (alerta == null)
                return NotFound($"Alerta con ID {id} no encontrada.");

            return Ok(alerta);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AlertaCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _alertaService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AlertaUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _alertaService.Update(dto);
            if (result == 0)
                return NotFound($"Alerta con ID {dto.IdAlerta} no encontrada.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _alertaService.Delete(id);
            if (!result)
                return NotFound($"Alerta con ID {id} no encontrada.");

            return NoContent();
        }
    }
}
