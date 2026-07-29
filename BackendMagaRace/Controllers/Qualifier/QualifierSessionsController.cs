using BackendMagaRace.Models;
using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendMagaRace.Controllers.Qualifier
{
    [ApiController]
    [Route("api/qualifiers")]
    [Authorize]
    public class QualifierSessionsController : ControllerBase
    {

        private readonly IQualifierService _service;


        public QualifierSessionsController(
            IQualifierService service)
        {
            _service = service;
        }



        [HttpPost("{id}/session/start")]
        public async Task<IActionResult> StartSession(Guid id)
        {

            var userId = Guid.Parse(
                User.FindFirst("sub")!.Value
            );


            var result =
                await _service.StartSession(
                    userId,
                    id);


            return Ok(result);
        }




    [HttpPost("session/{sessionId}/lap")]
    public async Task<IActionResult> SubmitLap(
    Guid sessionId,
    [FromBody] int timeMs)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("El token no contiene el identificador del usuario.");


        var userId = Guid.Parse(userIdClaim.Value);


        var result = await _service.SubmitLap(
            userId,
            sessionId,
            timeMs
        );


        return Ok(result);
    }


    [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSession(
            Guid sessionId)
        {

            var result =
                await _service.GetSession(sessionId);


            return Ok(result);
        }
    }
}