using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolRegistroController : ControllerBase
    {
        private readonly IRolRegistroService _rolRegistroService;

        public RolRegistroController(IRolRegistroService rolRegistroService)
        {
            _rolRegistroService = rolRegistroService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _rolRegistroService.GetAll();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rol = await _rolRegistroService.GetById(id);
            if (rol == null)
                return NotFound($"Rol de Registro con ID {id} no encontrado.");

            return Ok(rol);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolRegistroCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _rolRegistroService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RolRegistroUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rolRegistroService.Update(dto);
            if (result == 0)
                return NotFound($"Rol de Registro con ID {dto.IdRolRegistro} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rolRegistroService.Delete(id);
            if (!result)
                return NotFound($"Rol de Registro con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
