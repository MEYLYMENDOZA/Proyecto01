using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto01.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _service;

        public AreaController(IAreaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaListDTO>>> GetAll()
        {
            var areas = await _service.GetAll();
            return Ok(areas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AreaListDTO>> GetById(int id)
        {
            var area = await _service.GetById(id);
            if (area == null)
                return NotFound(new { message = $"햞ea con ID {id} no encontrada." });

            return Ok(area);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] AreaCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var id = await _service.Insert(dto);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update(int id, [FromBody] AreaUpdateDTO dto)
        {
            if (id != dto.IdArea)
                return BadRequest(new { message = "El ID en la URL no coincide con el ID del cuerpo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.Update(dto);
                if (result == 0)
                    return NotFound(new { message = $"햞ea con ID {id} no encontrada." });

                return Ok(new { message = "햞ea actualizada correctamente.", id = result });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.Delete(id);
                if (!result)
                    return NotFound(new { message = $"햞ea con ID {id} no encontrada." });

                return Ok(new { message = "햞ea eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
