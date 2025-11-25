using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoSolicitudCatalogoController : ControllerBase
    {
        private readonly ITipoSolicitudCatalogoService _tipoSolicitudCatalogoService;

        public TipoSolicitudCatalogoController(ITipoSolicitudCatalogoService tipoSolicitudCatalogoService)
        {
            _tipoSolicitudCatalogoService = tipoSolicitudCatalogoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _tipoSolicitudCatalogoService.GetAll();
            return Ok(tipos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _tipoSolicitudCatalogoService.GetById(id);
            if (tipo == null)
                return NotFound($"Tipo de Solicitud con ID {id} no encontrado.");

            return Ok(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoSolicitudCatalogoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _tipoSolicitudCatalogoService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TipoSolicitudCatalogoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tipoSolicitudCatalogoService.Update(dto);
            if (result == 0)
                return NotFound($"Tipo de Solicitud con ID {dto.IdTipoSolicitud} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tipoSolicitudCatalogoService.Delete(id);
            if (!result)
                return NotFound($"Tipo de Solicitud con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
