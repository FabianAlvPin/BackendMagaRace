using BackendMagaRace.Dtos;
using BackendMagaRace.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // POST /users/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            // Crear usuario y wallet en DB
            var user = await _userService.CreateAsync(
                email: dto.Email,
                username: dto.Username,
                password: dto.Password // opcional: hash dentro del servicio
            );

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Balance = user.Wallet.Balance
            });
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Balance = user.Wallet.Balance
            });
        }
    }
}
