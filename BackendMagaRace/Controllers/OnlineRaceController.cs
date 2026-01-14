using BackendMagaRace.Dtos.OnlineRace;
using BackendMagaRace.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("online-races")]
    public class OnlineRaceController : ControllerBase
    {
        private readonly OnlineRaceService _service;

        public OnlineRaceController(OnlineRaceService service)
        {
            _service = service;
        }

        // POST /online-races/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOnlineRaceDto dto)
        {
            var race = await _service.Create(dto);

            return Ok(new
            {
                race.RoomName,
                race.Id,
                race.TrackId,
                race.EntryCost,
                race.MaxPlayers,
                race.Laps
            });
        }

        // POST /online-races/{raceId}/join
        [HttpPost("{raceId}/join")]
        public async Task<IActionResult> Join(Guid raceId, [FromBody] JoinOnlineRaceDto dto)
        {
            await _service.Join(raceId, dto.UserId);
            return Ok();
        }
    }
}
