using BackendMagaRace.Dtos.Qualifier;
using BackendMagaRace.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("qualifier")]
    public class QualifierController : ControllerBase
    {
        private readonly QualifierService _service;

        public QualifierController(QualifierService service)
        {
            _service = service;
        }

        [HttpPost("join")]
        public async Task<IActionResult> Join(JoinQualifierDto dto)
        {
            var session = await _service.Join(dto.UserId, dto.QualifierEventId);
            return Ok(new QualifierSessionDto
            {
                ActiveUntil = session.ActiveUntil
            });
        }

        [HttpGet("ranking/{eventId}")]
        public async Task<IActionResult> Ranking(Guid eventId)
        {
            return Ok(await _service.GetRanking(eventId));
        }
    }

}
