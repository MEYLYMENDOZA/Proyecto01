using Microsoft.AspNetCore.Mvc;
using Proyecto01.CORE.Core.DTOs;
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

        // 1. GET ALL - Obtener todos los usuarios
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _usuarioService.GetAll();
            return Ok(users);
        }

        // 2. GET BY ID - Obtener un usuario por su ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _usuarioService.GetById(id);
            if (user == null)
                return NotFound($"Usuario con ID {id} no encontrado.");
            return Ok(user);
        }

        // 3. POST - Registrar/Crear un nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var id = await _usuarioService.SignUp(dto);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // 4. POST - Iniciar Sesión (SignIn)
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _usuarioService.SignIn(login.Correo, login.Password);

            if (user == null)
            {
                return Unauthorized("Correo o contraseña incorrectos.");
            }

            // Aquí puedes generar un token JWT si lo tienes implementado
            return Ok(user);
        }

        // 5. PUT - Actualizar un usuario existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar que el ID de la ruta coincida con el ID del DTO
            if (id != dto.IdUsuario)
                return BadRequest("El ID de la ruta no coincide con el ID del usuario.");

            try
            {
                var result = await _usuarioService.Update(dto);

                if (!result)
                    return NotFound($"Usuario con ID {dto.IdUsuario} no encontrado para actualizar.");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // 6. DELETE - Eliminar un usuario
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