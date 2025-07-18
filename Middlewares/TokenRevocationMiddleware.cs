using Listener_Yape.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
public class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRevocationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var iatClaim = context.User.FindFirst(JwtRegisteredClaimNames.Iat)?.Value;

            if (iatClaim != null && long.TryParse(iatClaim, out long iatUnix))
            {
                var tokenIssuedAt = DateTimeOffset.FromUnixTimeSeconds(iatUnix);

                var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var ultimaRevocacion = db.TokenRevocados
                    .Where(r => r.UsuarioId == userId)
                    .OrderByDescending(r => r.FechaRevocado)
                    .FirstOrDefault();

                if (ultimaRevocacion != null && tokenIssuedAt <= ultimaRevocacion.FechaRevocado)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token revocado");
                    return;
                }
            }


        }

        await _next(context);
    }
}
