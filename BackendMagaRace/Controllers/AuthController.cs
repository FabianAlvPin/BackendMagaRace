using BackendMagaRace.Data;
using BackendMagaRace.Dtos;
using BackendMagaRace.Models;
using BackendMagaRace.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BackendMagaRace.Data;
namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwt;
        private readonly AppDbContext _context;

        public AuthController(
       UserService userService,
       JwtService jwt,
       AppDbContext context)
        {
            _userService = userService;
            _jwt = jwt;
            _context = context;
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
    [FromBody] string refreshToken)
        {
            var token =
                await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshToken);


            if (token == null)
                return Unauthorized("Refresh token inválido");


            if (token.Revoked)
                return Unauthorized("Refresh token revocado");


            if (token.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Refresh token expirado");


            var newAccessToken =
                _jwt.GenerateToken(
                    token.User.Id.ToString(),
                    token.User.Username
                );


            return Ok(new
            {
                accessToken = newAccessToken
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username y Password son requeridos");

            var user = await _userService.GetByUsernameAsync(request.Username);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized("Usuario o contraseña incorrectos");

            var token = _jwt.GenerateToken(
    user.Id.ToString(),
    user.Username
);


            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                Token = Convert.ToBase64String(
                    RandomNumberGenerator.GetBytes(64)
                ),

                ExpiresAt = DateTime.UtcNow.AddDays(30),

                Revoked = false
            };


            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();



            return Ok(new
            {
                accessToken = token,

                refreshToken = refreshToken.Token,

                user = new
                {
                    user.Id,
                    user.Username,
                    user.Wallet.Balance
                }
            });
        }

        // --- Helpers ---
        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return hash == storedHash;
        }
    }


    // DTO para login
    public class LoginRequestDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

}
