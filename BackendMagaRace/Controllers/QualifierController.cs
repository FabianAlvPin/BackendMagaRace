using BackendMagaRace.Dtos.OnlineRace;
using BackendMagaRace.Dtos.Qualifier;
using BackendMagaRace.Models;
using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static BackendMagaRace.Services.QualifierService;


namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("qualifier")]
    public class QualifierController : ControllerBase
    {
        private readonly IQualifierService _service;


        public QualifierController(IQualifierService service)
        {
            _service = service;
        }



        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinQualifierDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                    return Unauthorized("El token no contiene el identificador del usuario.");

                var userId = Guid.Parse(userIdClaim.Value);

                var session = await _service.Join(userId, dto.QualifierEventId);

                return Ok(new QualifierSessionDto
                {
                    Id = session.Id,
                    ActiveUntil = session.ActiveUntil
                });
            }
            catch (BusinessException ex)
            {
                // Error esperado (saldo, evento, wallet, etc.)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Error inesperado
                return StatusCode(500, ex.ToString());
            }
        }
        // verificar si tiene sesión activa
        [HttpGet("session/{userId}/{eventId}")]
        public async Task<IActionResult> GetSession(Guid userId, Guid eventId)
        {
            var session = await _service.GetActiveSession(userId, eventId);

            if (session == null)
                return Ok(new { active = false });

            return Ok(new
            {
                active = true,
                expires = session.ActiveUntil
            });
        }

        // enviar vuelta
        [HttpPost("session/{sessionId}/lap")]
        public async Task<IActionResult> SubmitLap(
            Guid sessionId,
            [FromBody] int timeMs)
        {
            var userId = Guid.Parse(
                User.FindFirst("sub")!.Value
            );


            var result = await _service.SubmitLap(
                userId,
                sessionId,
                timeMs
            );


            return Ok(result);
        }

        // ranking
        [HttpGet("ranking/{eventId}")]
        public async Task<IActionResult> Ranking(Guid eventId)
        {
            return Ok(await _service.GetRanking(eventId));
        }

        // posición del jugador
        [HttpGet("position/{eventId}/{userId}")]
        public async Task<IActionResult> PlayerPosition(Guid eventId, Guid userId)
        {
            return Ok(await _service.GetPlayerPosition(eventId, userId));
        }
        [HttpPost("event")]
        public async Task<IActionResult> CreateEvent(CreateQualifierEventDto dto)
        {
            var ev = await _service.CreateEvent(dto);

            return Ok(ev);
        }
    }

}
