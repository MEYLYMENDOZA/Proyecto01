using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesSistemaController : ControllerBase
    {
        private readonly IRolesSistemaService _rolesSistemaService;

        public RolesSistemaController(IRolesSistemaService rolesSistemaService)
        {
            _rolesSistemaService = rolesSistemaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _rolesSistemaService.GetAll();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rol = await _rolesSistemaService.GetById(id);
            if (rol == null)
                return NotFound($"Rol de Sistema con ID {id} no encontrado.");

            return Ok(rol);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolesSistemaCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _rolesSistemaService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RolesSistemaUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rolesSistemaService.Update(dto);
            if (result == 0)
                return NotFound($"Rol de Sistema con ID {dto.IdRolSistema} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rolesSistemaService.Delete(id);
            if (!result)
                return NotFound($"Rol de Sistema con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
