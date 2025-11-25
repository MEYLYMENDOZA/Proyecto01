using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisoController : ControllerBase
    {
        private readonly IPermisoService _permisoService;

        public PermisoController(IPermisoService permisoService)
        {
            _permisoService = permisoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permisos = await _permisoService.GetAll();
            return Ok(permisos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var permiso = await _permisoService.GetById(id);
            if (permiso == null)
                return NotFound($"Permiso con ID {id} no encontrado.");

            return Ok(permiso);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermisoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _permisoService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PermisoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permisoService.Update(dto);
            if (result == 0)
                return NotFound($"Permiso con ID {dto.IdPermiso} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _permisoService.Delete(id);
            if (!result)
                return NotFound($"Permiso con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
