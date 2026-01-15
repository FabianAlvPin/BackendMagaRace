using BackendMagaRace.Dtos;
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
            var race = await _service.Create( dto);

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


        [HttpPost("{raceId}/start")]
        public async Task<IActionResult> StartRace(Guid raceId, [FromBody] StartRaceDto dto)
        {
            try
            {
                var race = await _service.StartRaceAsync(raceId, dto.UserId);

                return Ok(new
                {
                    race.Id,
                    race.Status,
                    race.StartedAt
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("{raceId}/join")]
        public async Task<IActionResult> JoinRace(
            Guid raceId,
            [FromBody] JoinOnlineRaceDto dto)
        {
            try
            {
                await _service.JoinRaceAsync(raceId, dto.UserId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
