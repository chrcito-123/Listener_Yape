using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Listener_Yape.Data;
using Listener_Yape.Models;

namespace Listener_Yape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuscripcionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuscripcionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/suscripciones
        [HttpGet]
        public async Task<IActionResult> GetSuscripcion()
        {
            var username = User.Identity.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null) return Unauthorized();

            var suscripcion = await _context.Suscripciones
                .Where(s => s.UsuarioId == usuario.Id)
                .OrderByDescending(s => s.FechaInicio)
                .FirstOrDefaultAsync();

            if (suscripcion == null)
            {
                return Ok(new { message = "No tienes suscripción activa" });
            }

            return Ok(suscripcion);
        }

        // POST: api/suscripciones
        [HttpPost]
        public async Task<IActionResult> CrearSuscripcion([FromBody] Suscripcion request)
        {
            var username = User.Identity.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null) return Unauthorized();

            request.UsuarioId = usuario.Id;
            request.FechaInicio = DateTime.UtcNow;
            request.FechaFin = DateTime.UtcNow.AddDays(30);  // Ejemplo: 30 días
            request.Estado = "Activa";
            request.CreatedAt = DateTime.UtcNow;

            _context.Suscripciones.Add(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}
