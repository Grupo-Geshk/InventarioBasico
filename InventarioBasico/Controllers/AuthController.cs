using InventarioBasico.Data;
using InventarioBasico.Dto.Auth;
using InventarioBasico.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventarioBasico.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly InventarioDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(InventarioDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Validar que no exista el usuario
            var exists = await _context.Usuarios.AnyAsync(u => u.Username == dto.Username);
            if (exists) return BadRequest("El nombre de usuario ya existe");

            // Crear usuario (de momento, password sin hash para prueba)
            var user = new Usuario
            {
                Username = dto.Username,
                PasswordHash = dto.Password,  // luego se mejora con hashing
                Email = dto.Email
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return Unauthorized("Usuario no encontrado");

            // Para prueba simple: password en texto (¡luego agregar hash!)
            if (user.PasswordHash != dto.Password) return Unauthorized("Contraseña incorrecta");

            // Generar token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }
    }
}
