using Listener_Yape.Data;
using Listener_Yape.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AplicacionesController : ControllerBase
{
    private readonly AppDbContext _context;
    public AplicacionesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var apps = await _context.Aplicaciones.ToListAsync();
        return Ok(apps);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Aplicacion request)
    {
        _context.Aplicaciones.Add(request);
        await _context.SaveChangesAsync();
        return Ok(request);
    }
}
