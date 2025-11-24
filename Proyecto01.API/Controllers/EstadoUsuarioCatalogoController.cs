using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoUsuarioCatalogoController : ControllerBase
    {
        private readonly IEstadoUsuarioCatalogoService _estadoUsuarioCatalogoService;

        public EstadoUsuarioCatalogoController(IEstadoUsuarioCatalogoService estadoUsuarioCatalogoService)
        {
            _estadoUsuarioCatalogoService = estadoUsuarioCatalogoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var estados = await _estadoUsuarioCatalogoService.GetAll();
            return Ok(estados);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var estado = await _estadoUsuarioCatalogoService.GetById(id);
            if (estado == null)
                return NotFound($"Estado de Usuario con ID {id} no encontrado.");

            return Ok(estado);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EstadoUsuarioCatalogoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _estadoUsuarioCatalogoService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EstadoUsuarioCatalogoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _estadoUsuarioCatalogoService.Update(dto);
            if (result == 0)
                return NotFound($"Estado de Usuario con ID {dto.IdEstadoUsuario} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estadoUsuarioCatalogoService.Delete(id);
            if (!result)
                return NotFound($"Estado de Usuario con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
