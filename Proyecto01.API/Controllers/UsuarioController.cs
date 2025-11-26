using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs; // Necesario para LoginDTO y los demás
using Proyecto01.CORE.Core.Interfaces;

namespace Proyecto01.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UserController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // 1. GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _usuarioService.GetAll();
            return Ok(users);
        }

        // 2. GET BY ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _usuarioService.GetById(id);
            if (user == null)
                return NotFound($"Usuario con ID {id} no encontrado.");
            return Ok(user);
        }

        // 3. POST (Registrar/Crear)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _usuarioService.SignUp(dto);

            // CORRECCIÓN: nombre del método CreateAtAction corregido de GetByIo a GetById
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // 4. POST (Iniciar Sesión)
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Usa el Correo del LoginDTO para el login seguro
            var user = await _usuarioService.SignIn(login.Correo, login.Password);

            if (user == null)
            {
                return Unauthorized("Correo o contraseña incorrectos.");
            }
            // Aquí puedes generar un token JWT si lo tienes implementado
            return Ok(user);
        }

        // 5. PUT (Actualizar)
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UsuarioResponseDTO dto) // Usar UsuarioUpdateDTO es más limpio
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _usuarioService.Update(dto);

            if (result == 0)
                return NotFound($"Usuario con ID {dto.IdUsuario} no encontrado para actualizar.");

            return NoContent();
        }

        // 6. DELETE (Eliminar)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _usuarioService.Delete(id);

            if (!success)
                return NotFound($"Usuario con ID {id} no encontrado para eliminar.");

            return NoContent();
        }
    }
}