using Microsoft.AspNetCore.Mvc;
using Listener_Yape.Data;
using Listener_Yape.Models;

namespace Listener_Yape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            return Ok(usuarios);
        }

        // POST: api/Usuarios
        [HttpPost]
        public IActionResult CreateUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Username))
            {
                return BadRequest("Datos de usuario inválidos");
            }

            usuario.CreatedAt = DateTime.UtcNow;
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, usuario);
        }
    }
}
