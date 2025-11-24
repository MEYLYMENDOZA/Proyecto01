using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto01.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalController : ControllerBase
    {
        private readonly IPersonalService _service;

        public PersonalController(IPersonalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonalListDTO>>> GetAll()
        {
            var personales = await _service.GetAll();
            return Ok(personales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonalListDTO>> GetById(int id)
        {
            var personal = await _service.GetById(id);
            if (personal == null)
                return NotFound(new { message = $"Personal con ID {id} no encontrado." });

            return Ok(personal);
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<PersonalListDTO>> GetByIdUsuario(int idUsuario)
        {
            var personal = await _service.GetByIdUsuario(idUsuario);
            if (personal == null)
                return NotFound(new { message = $"Personal del usuario {idUsuario} no encontrado." });

            return Ok(personal);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] PersonalCreateDTO dto)
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
        public async Task<ActionResult<int>> Update(int id, [FromBody] PersonalUpdateDTO dto)
        {
            if (id != dto.IdPersonal)
                return BadRequest(new { message = "El ID en la URL no coincide con el ID del cuerpo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.Update(dto);
                if (result == 0)
                    return NotFound(new { message = $"Personal con ID {id} no encontrado." });

                return Ok(new { message = "Personal actualizado correctamente.", id = result });
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
                    return NotFound(new { message = $"Personal con ID {id} no encontrado." });

                return Ok(new { message = "Personal eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
