using BackendMagaRace.Dtos;
using BackendMagaRace.Services;
using Microsoft.AspNetCore.Mvc;
using static BackendMagaRace.Services.UserService;

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
            try
            {
                var user = await _userService.CreateAsync(
                    dto.Email,
                    dto.Username,
                    dto.Password
                );

                return Ok(new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Balance = user.Wallet.Balance
                });
            }
            catch (DomainException ex)
            {
                return StatusCode(ex.StatusCode, new
                {
                    error = ex.Message
                });
            }
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
