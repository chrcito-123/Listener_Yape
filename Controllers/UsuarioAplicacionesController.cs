using Listener_Yape.Data;
using Listener_Yape.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsuarioAplicacionesController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsuarioAplicacionesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var apps = await _context.UsuarioAplicaciones
            .Include(x => x.Aplicacion)
            .Where(x => x.UsuarioId == userId)
            .Select(x => new { x.AplicacionId, x.Aplicacion.Nombre, x.Aplicacion.PackageName })
            .ToListAsync();
        return Ok(apps);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] int[] appIds)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var actuales = await _context.UsuarioAplicaciones.Where(x => x.UsuarioId == userId).ToListAsync();
        _context.UsuarioAplicaciones.RemoveRange(actuales);

        var nuevas = appIds.Select(id => new UsuarioAplicacion
        {
            UsuarioId = userId,
            AplicacionId = id,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _context.UsuarioAplicaciones.AddRange(nuevas);
        await _context.SaveChangesAsync();
        return Ok(nuevas);
    }

    // DELETE: api/usuarioaplicaciones/2
    [HttpDelete("{aplicacionId}")]
    public async Task<IActionResult> QuitarAplicacion(int aplicacionId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var relacion = await _context.UsuarioAplicaciones
            .FirstOrDefaultAsync(ua => ua.UsuarioId == userId && ua.AplicacionId == aplicacionId);

        if (relacion == null)
            return NotFound(new { message = "La aplicación no está asociada al usuario" });

        _context.UsuarioAplicaciones.Remove(relacion);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Aplicación eliminada correctamente" });
    }
}
