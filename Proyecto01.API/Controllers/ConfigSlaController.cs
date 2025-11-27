using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigSlaController : ControllerBase
    {
        private readonly IConfigSlaService _configSlaService;

        public ConfigSlaController(IConfigSlaService configSlaService)
        {
            _configSlaService = configSlaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configSlas = await _configSlaService.GetAll();
            return Ok(configSlas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var configSla = await _configSlaService.GetById(id);
            if (configSla == null)
                return NotFound($"ConfigSla con ID {id} no encontrada.");

            return Ok(configSla);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConfigSlaCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _configSlaService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] List<ConfigSlaUpdateDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest("La lista de configuraciones no puede estar vacía.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var dto in dtos)
            {
                await _configSlaService.Update(dto);
            }
            
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _configSlaService.Delete(id);
            if (!result)
                return NotFound($"ConfigSla con ID {id} no encontrada.");

            return NoContent();
        }
    }
}
