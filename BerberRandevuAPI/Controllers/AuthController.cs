using BerberRandevuAPI.Data;
using BerberRandevuAPI.Models;
using BerberRandevuAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BerberRandevuAPI.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BerberContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BerberContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser(UserRegisterDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Bu mail zaten kayıtlı");
            }
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Kullanıcı başarıyla kayıt oldu ");
        }
        [HttpPost("register/barber")]
        public async Task<IActionResult> RegisterBarber(BarberRegisterDTO dto)

        {
            if (await _context.Users.AnyAsync(b => b.Email == dto.Email))
                return BadRequest("Bu e mail zaten kayıtlı");

            var barber = new Barber
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();
            return Ok("Berber başarıyla kayıt oldu");
        }

        [HttpPost("login/user")]
        public async Task<IActionResult> LoginUser(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Email veya şifre yanlış");
            var extraClaims = new Dictionary<string, string>
            {
                { "userId",user.UserId.ToString() }
            };
            var token = GenerateJwtToken(user.Email, "User", extraClaims);
            return Ok(new { token });
        }

        [HttpPost("login/barber")]
        public async Task<IActionResult> LoginBarber(LoginDTO dto)
        {
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.Email == dto.Email);
            if (barber == null || !BCrypt.Net.BCrypt.Verify(dto.Password, barber.PasswordHash))
                return Unauthorized("Email veya şifre yanlış");
            var extraClaims = new Dictionary<string,string>
            {
                {"barberId",barber.BarberId.ToString()}
            };
            var token = GenerateJwtToken(barber.Email, "Barber", extraClaims);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string email, string role, Dictionary<string, string> extraClaims)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(ClaimTypes.Role, role),
    };

            foreach (var pair in extraClaims)
            {
                claims.Add(new Claim(pair.Key, pair.Value));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

