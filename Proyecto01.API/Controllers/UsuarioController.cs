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
            // Nota: Este endpoint debe estar protegido para solo Administradores.
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

        // 3. POST - Registrar/Crear un nuevo usuario (SignUp)
        // La ruta es /api/User
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Aquí el servicio debe hashear la contraseña
                var id = await _usuarioService.SignUp(dto);
                // Retorna 201 Created y la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (ArgumentException ex)
            {
                // Manejar errores de validación específica o datos inválidos (e.g., ID de rol/estado no existe)
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Manejar conflictos como el correo o username ya existen
                return Conflict(new { message = ex.Message });
            }
        }

        // 4. POST - Iniciar Sesión (SignIn)
        // La ruta es /api/User/SignIn
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Nota: El LoginDTO debe contener Correo y Password
            var user = await _usuarioService.SignIn(login.Correo, login.Password);

            if (user == null)
            {
                // 401 Unauthorized
                return Unauthorized("Correo o contraseña incorrectos, o usuario inactivo.");
            }

            // Retorna 200 OK con los datos del usuario (excluyendo el hash)
            return Ok(user);
        }

        // 5. PUT - Actualizar un usuario existente
        // La ruta es /api/User/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // CRÍTICO: Asegurar que el ID de la ruta y el cuerpo coincidan
            if (id != dto.IdUsuario)
                return BadRequest(new { Message = "El ID de la ruta no coincide con el ID del usuario a actualizar." });

            try
            {
                var result = await _usuarioService.Update(dto);

                if (!result)
                {
                    // Si el servicio devuelve false, el usuario no fue encontrado
                    return NotFound($"Usuario con ID {id} no encontrado para actualizar.");
                }

                // 204 No Content: Éxito en la actualización sin devolver contenido
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                // Manejar errores de datos inválidos (e.g., formato de correo)
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Manejar errores de duplicidad (e.g., intentar cambiar el correo a uno existente)
                return Conflict(new { message = ex.Message });
            }
        }

        // 6. DELETE - Desactivar un usuario (Soft Delete)
        // NOTA: Este endpoint NO elimina físicamente el usuario
        // Solo lo marca como INACTIVO para mantener la integridad de datos
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _usuarioService.Delete(id);

            if (!success)
                // 404 Not Found
                return NotFound($"Usuario con ID {id} no encontrado.");

            // 204 No Content: Éxito en la eliminación sin devolver contenido
            return Ok(new 
            { 
                message = "Usuario desactivado exitosamente. El usuario ha sido marcado como inactivo.",
                id = id,
                note = "Los datos del usuario se mantienen por integridad referencial."
            });
        }
    }
}