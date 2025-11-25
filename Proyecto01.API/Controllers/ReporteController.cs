using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reportes = await _reporteService.GetAll();
            return Ok(reportes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reporte = await _reporteService.GetById(id);
            if (reporte == null)
                return NotFound($"Reporte con ID {id} no encontrado.");

            return Ok(reporte);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReporteCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _reporteService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ReporteUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reporteService.Update(dto);
            if (result == 0)
                return NotFound($"Reporte con ID {dto.IdReporte} no encontrado.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reporteService.Delete(id);
            if (!result)
                return NotFound($"Reporte con ID {id} no encontrado.");

            return NoContent();
        }
    }
}
