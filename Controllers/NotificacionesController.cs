using Listener_Yape.Data;
using Listener_Yape.Models;
using Listener_Yape.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Listener_Yape.Models.Requests;

namespace Listener_Yape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificacionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarNotificacion([FromBody] NotificacionRequest req)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var suscripcionActiva = await _context.Suscripciones
                .AnyAsync(s => s.UsuarioId == userId &&
                               s.FechaInicio <= DateTime.UtcNow &&
                               s.FechaFin >= DateTime.UtcNow &&
                               s.Estado == "Activo");

            if (!suscripcionActiva)
            {
                return StatusCode(403, new { message = "Debes tener una suscripción activa" });
            }

            var app = await _context.Aplicaciones.FirstOrDefaultAsync(a => a.PackageName == req.PackageName);
            if (app == null)
            {
                return BadRequest(new { message = "La aplicación no está registrada" });
            }

            var (persona, monto) = NotificationParser.Parse(req.PackageName, req.MensajeCompleto);

            var notificacion = new Notificacion
            {
                UsuarioId = userId,
                AplicacionId = app.Id,
                FechaNotificacion = req.FechaNotificacion,
                CreatedAt = DateTime.UtcNow,
                MensajeCompleto = req.MensajeCompleto,
                Persona = persona,
                Monto = monto
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(notificacion);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificaciones()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var suscripcionActiva = await _context.Suscripciones
                .AnyAsync(s => s.UsuarioId == userId &&
                               s.FechaInicio <= DateTime.UtcNow &&
                               s.FechaFin >= DateTime.UtcNow &&
                               s.Estado == "Activo");

            if (!suscripcionActiva)
            {
                return StatusCode(403, new { message = "Debes tener una suscripción activa para ver tus notificaciones" });
            }

            var notificaciones = await _context.Notificaciones
                .Where(n => n.UsuarioId == userId)
                .OrderByDescending(n => n.FechaNotificacion)
                .ToListAsync();

            return Ok(notificaciones);
        }

        [HttpPost("{id}/visto")]
        public async Task<IActionResult> MarcarVisto(int id, [FromBody] bool visto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var noti = await _context.Notificaciones.FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == userId);
            if (noti == null) return NotFound();

            noti.Vista = visto;
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Notificación actualizada a vista = {visto}" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarNotificacion(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == userId);

            if (notificacion == null)
                return NotFound(new { message = "Notificación no encontrada" });

            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notificación eliminada correctamente" });
        }



    }
}
