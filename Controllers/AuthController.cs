using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Listener_Yape.Data;
using Listener_Yape.Models;
using BCrypt.Net;

namespace Listener_Yape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        private bool ValidarTokenDesdeHeader(out IActionResult error)
        {
            error = null;
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                error = StatusCode(403, new { message = "Token de autorización requerido" });
                return false;
            }

            var tokenEnviado = authHeader.Substring("Bearer ".Length).Trim();
            var tokenEsperado = _dbContext.Parametros
                .Where(p => p.Clave == "ApiKey")
                .Select(p => p.Valor)
                .FirstOrDefault();

            if (tokenEnviado != tokenEsperado)
            {
                error = StatusCode(403, new { message = "Token inválido" });
                return false;
            }

            return true;
        }


        // POST: api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            if (!ValidarTokenDesdeHeader(out var error)) return error;
            if (_dbContext.Usuarios.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Usuario ya existe" });
            }

            var user = new Usuario
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };

            _dbContext.Usuarios.Add(user);
            _dbContext.SaveChanges();

            // Trial automático
            var trial = new Suscripcion
            {
                UsuarioId = user.Id,
                Tipo = "Trial",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(1),
                Estado = "Activo",
                CreatedAt = DateTime.Now
            };

            _dbContext.Suscripciones.Add(trial);
            _dbContext.SaveChanges();

            return Ok(new { message = "Usuario registrado con trial" });
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ValidarTokenDesdeHeader(out var error)) return error;
            var user = _dbContext.Usuarios.FirstOrDefault(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            var suscripcion = _dbContext.Suscripciones
                .Where(s => s.UsuarioId == user.Id && s.FechaInicio <= DateTime.Now && s.FechaFin >= DateTime.Now && s.Estado == "Activo")
                .FirstOrDefault();

            if (suscripcion == null)
            {
                return Unauthorized(new { message = "No tiene una suscripción activa" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        [Authorize]
        [HttpPost("revocar-tokens")]
        public IActionResult RevocarTokens()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var revocado = new TokenRevocado { UsuarioId = userId };
            _dbContext.TokenRevocados.Add(revocado);
            _dbContext.SaveChanges();

            return Ok(new { message = "Todos los tokens anteriores han sido revocados" });
        }

        // GET: api/auth/perfil
        [Authorize]
        [HttpGet("perfil")]
        public IActionResult Perfil()
        {
            var username = User.Identity.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(new { mensaje = $"Bienvenido {username} (ID: {userId}), estás autenticado" });
        }

        [Authorize]
        [HttpPut("cambiar-password")]
        public IActionResult CambiarPassword([FromBody] CambiarPasswordRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _dbContext.Usuarios.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            if (!BCrypt.Net.BCrypt.Verify(request.PasswordActual, user.PasswordHash))
                return BadRequest(new { message = "La contraseña actual es incorrecta" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordNuevo);
            _dbContext.SaveChanges();

            return Ok(new { message = "Contraseña actualizada correctamente" });
        }

        [Authorize]
        [HttpGet("link-acortado")]
        public IActionResult GenerarLinkAcortado()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var username = User.Identity.Name;
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token no encontrado en cabecera Authorization" });

            var claveCifrado = _dbContext.Parametros
                .Where(p => p.Clave == "cifrado")
                .Select(p => p.Valor)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(claveCifrado))
                return StatusCode(500, new { message = "Clave de cifrado no configurada" });

            var usuarioCifrado = Cifrar(username, claveCifrado);
            var tokenCifrado = Cifrar(token, claveCifrado);

            var link = $"https://noticheckapp.com?8iie8={usuarioCifrado}&t94fh={tokenCifrado}";
            return Ok(new { link });
        }


        private string Cifrar(string textoPlano, string clave)
        {
            var key = Encoding.UTF8.GetBytes(clave.PadRight(32).Substring(0, 32));
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(textoPlano);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var resultado = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, resultado, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, resultado, aes.IV.Length, cipherBytes.Length);

            return Convert.ToBase64String(resultado).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        [Authorize]
        [HttpPut("cambiar-nombre")]
        public IActionResult CambiarNombre([FromBody] CambiarNombreRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _dbContext.Usuarios.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            user.Nombre = request.Nombre;
            _dbContext.SaveChanges();

            return Ok(new { message = "Nombre actualizado correctamente", nombre = user.Nombre });
        }

        public class CambiarNombreRequest
        {
            public string Nombre { get; set; }
        }

        public class CambiarPasswordRequest
        {
            public string PasswordActual { get; set; }
            public string PasswordNuevo { get; set; }
        }


        private string GenerateJwtToken(Usuario user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                signingCredentials: creds
            // sin expires, será token infinito (hasta revocación)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
